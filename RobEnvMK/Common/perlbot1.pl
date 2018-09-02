#!/usr/bin/perl

my @radar;
my $rrr=1, $size=0, $step=0;
my $pi = 3.14159265359;
my $ef = $step*2;
my $deg = $pi/180;
my $TRCHANDLE = "trchandle";
my $mvd = 0;

$SIG{INT} = sub {
    Trace("SIGINT");
	close (TRCHANDLE);
};

$SIG{TERM} = sub {
    Trace("SIGTERM");
	close (TRCHANDLE);
};

$SIG{ABRT} = sub {
    Trace("SIGABRT");
	close (TRCHANDLE);
};

sub Trace
{
   my $text = $_[0];

   open(TRCHANDLE,">>perlbot1.trc") or die;
   printf TRCHANDLE "$text\n";
#   close TRCHANDLE
}

# Check of bot was killed in the previous step.
sub Killed
{
   print "?Killed\n";
   sleep (0.1);
   chomp($resp = <STDIN>);
   sleep (0.1);
   if ($resp !~ m/true/ && $resp !~ m/false/) {
      Trace("Killed: no response from environment. Assuming bot is killed.");
      $resp='true';
   }
 
   return $resp;
}

# Obtain radar reading.
sub Radar
{
   print "?Radar\n";
   sleep (0.1);
   chomp($data = <STDIN>);
   sleep (0.1);
   @values = split (/ /, $data);

   return @values;
}

# Read radar to @radar array if there is a need.
# Setup global variables based on that reading.
sub ReadRadar
{
   if ($rrr != 0) {
      Trace("ReadRadar");
      @radar = Radar ();
      $rrr = 0;
   }
}

# Make a single step, check if killed.
sub Move
{
   my $k;
   print "Move\n";
   sleep (0.1);
   $rrr = 1;
   $k=Killed();
   if ($k =~ m/true/) {
      Trace("Bot killed by collision with the obstacle.");
   }
   $mvd = 1;

   return $k;
}

sub Kill
{
   print "Kill\n";
   sleep(0.1);
}

# Drive the bot ahead to the nearest wall
sub Go
{
   my $n = 0;
   my $k = Killed();
   my $i;
   my $m = 1;
   my $dst = 1000;
   ReadRadar;
   Trace("Go: radar[180]=$radar[180]");
   $m = 1;
   for ($i=110; $i<=250; $i=$i+1) {
      if ($radar[$i] <= ($size+$ef)) {
         $m = 0;
		 last;
	  }
	  if ($dst > $radar[$i]) {
	     $dst = $radar[$i];
	  }
   }
   if ($m != 0 && $dst > $size*2+$ef) {
      $i = int($dst/$step) - 3;
	  Trace("Distance to go: $dst, Steps to go: $i");
	  while ($i > 0 && $k eq 'false') {
        $k=Move();
	    if ($k eq 'true') { 
		   Trace("Bot killed at step: $n"); 
		   last; 
		}
        $n=$n+1;
		$i=$i-1;
	  }
   } 
   Trace("Go: moved $n steps.");
}

# Turn the bot the specified angle (radians).
sub Turn
{
   print "Turn $_[0]\n";
   sleep (0.1);
   $rrr = 1;
}

# Turn the bot to the longest opening ahead
sub Turn2Opening
{
   my $max = 0;
   my $idx = 0;
   my $angle = 0;
   ReadRadar;
   for ($i=100; $i<=260; $i=$i+1) {
      if ($radar[$i] > $max) {
         $max = $radar[$i];
         $idx = $i;
      }
   }
   if ($idx > 0) {
      $angle = ($deg)*($idx-180);
      Trace ("Turn2Opening: angle=$angle");
      if ($angle != 0) {
         Turn $angle;
      }
   }
}
 
# Get the bot's size from environment.
sub GetSize
{
   print "?Size\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}
 
# Get the step's length from environment. 
sub GetStep
{
   print "?Step\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}

# Calculate average of radar readings in specified slice.
sub RadarAvg
{
   my $frm = $_[0];
   my $to  = $_[1];
   my $i=0, $j=0, $awg=0;
   for ($i=$frm; $i<$to; $i=$i+1) {
      $awg = $awg + $radar[$i];
	  $j=$j+1;
   }
   $awg = $awg/$j;

   return $awg;
}

# Disable buffering
$|=1;

$size = GetSize ();
$step = GetStep ();
$ef = $step*4;
$rrr = 1;

my $iskilled = Killed ();
my $maxstuck = 12;
my $numstuck = $maxstuck;

Trace("-------------------------------------------------------------");
Trace("Starting bot: size=$size, step=$step");
while ($iskilled =~ m/false/) {
	
   $mvd = 0;
   ReadRadar;

   Turn2Opening;
   Go;
   ReadRadar;
   if ($mvd == 0) {
      my $awg = 0, $nawg = 0;
      my $i=0, $j=0;
      my $slice=36;
      my $slc = -(360/$slice/2 + 1);
	  my $n=0;
	  my @slctb;
      for($i=0; $i<360; $i=$i+$slice) {
	     $nawg = RadarAvg($i, $i+$slice);
		 if ($nawg > $awg) {
		    $awg = $nawg;
			push @slctb, $slc;
		 }
         $slc = $slc + 1;
	  }
	  while (@slctb && $mvd == 0) {
	     $n = pop @slctb;
	     my $ang = $n*$deg*($slice/2);
	     Trace ("Recovering from lock-up: Turn $ang"); 
	     Turn $ang;
	     Go;
      }
      if ($mvd == 0) {
         $numstuck = $numstuck - 1;
	     Trace("Bot is stuck, # of tries left: $numstuck");
	     if ($numstuck == 0) {
            Trace("Bot is stuck. Misson aborted (commiting suicide).");
	        Kill;
	     }
		 Turn $deg*36;
		 Go;
      } else {
         $numstuck = $maxstuck;
	  }
   } else {
      $numstuck = $maxstuck;
   }
   $iskilled = Killed ();
   sleep(0.1);
};
Trace("Ouch! I am killed.");
