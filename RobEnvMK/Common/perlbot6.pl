#!/usr/bin/perl
# This bot follows the wall to the right.
#
use List::Util qw(min max);

my @radar;
my @pradar;
my $rrr = 1;
my $pi = 3.14159265359;
my $step = 0.05, $size = 2, $ef = $step * 4;
my $tl = $pi / 2;  # angle to turn left
my $tr = -$tl;   # angle to turn right
my $TRCHANDLE = "trchandle";
my $iskilled = 'false';
my $er = 0.0;

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

   open(TRCHANDLE,">>perlbot6.trc") or die;
   printf TRCHANDLE "$text\n";
}

sub Goal
{
   print "?Eval $pi $size\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   Trace("Goal=$resp");

   # In the vicinity of the exit
   if ($resp > -60) {
      Trace("SUCCESS! Goal reached!");
   }

   return $resp;
}

sub DumpRadar
{
   Trace("DumpRadar:");
   for ($i = 0; $i < 360; $i++) {
      Trace("Radar[$i] = $radar[$i]");   
   }
   Goal;
}

sub Move
{
   print "Move\n";
   $rrr = 1;
   sleep (0.1);
   Goal;
}
 
sub Turn
{
   print "Turn $_[0]\n";
   $rrr = 1;
   sleep (0.1);
}

sub Suicide
{
   Trace("Commit suicide (Kill).");
   print "Kill\n";
   sleep(0.1);
}

sub GetSize
{
   print "?Size\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   Trace("Size=$resp");

   return $resp;
}
 
sub GetStep
{
   print "?Step\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   Trace("Step=$resp");

   return $resp;
}

sub Killed
{
   print "?Killed\n";
   sleep (0.1);
   chomp($resp = <STDIN>);
   $iskilled = $resp;
   sleep (0.1);
   if ($resp !~ m/true/ && $resp !~ m/false/) {
      Trace("Killed: no response from environment. Assuming bot is killed.");
      $resp='true';
   }

   if ($resp eq 'true') {
      Trace("Killed: robot has died.");
      Trace("Killed: dumping previously read radar:");
      DumpRadar;
      Trace("Killed: reading and dumping current radar:");
      ReadRadar;
      DumpRadar;
   }
 
   return $resp;
}

sub Radar
{
   print "?Radar\n";
   sleep (0.1);
   chomp($data = <STDIN>);
   sleep (0.1);
   @values = split (/ /, $data);

   return @values;
}

sub PartRadar
{
   print "?PRadar $_[0] $_[1]\n";
   sleep (0.1);
   chomp($data = <STDIN>);
   sleep (0.1);
   my @values = split (/ /, $data);

   #my $prsize = @values;
   #my $maxidx = $#values;

   #Trace("PartRadar: size = $prsize, max index = $maxidx");
   Trace("PartRadar");

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

# approach the wall ahead
sub Go2Wall
{
   my $n = 0;
   my $i = 0;
   my $dist = 1000.0;
   my $angle = 0.0;

   if ($rrr == 0) {
      for ($i = 0; $i < 180; $i++) {
         if ($radar[$i] < $dist) {
            $dist = $radar[$i];
            $angle = - (180 - $i) * $pi / 180;
         }
      }
   } else {
      @pradar = PartRadar(0, 180);
      for ($i = 0; $i < 180; $i++) {
         if ($pradar[$i] < $dist) {
            $dist = $pradar[$i];
            $angle = - (180 - $i) * $pi / 180;
         }
      }
   }
   Trace("Go2Wall: Turn to closest wall to the right, angle = $angle");
   Turn $angle;
   Killed;
   while ($iskilled eq 'false')
   {
      my $offs = 120;
      @pradar = PartRadar($offs, 121);
      # look carefully before moving
      if ($pradar[180-$offs] > ($size + $er) 
          && $pradar[175-$offs] > ($size + $er) 
          && $pradar[185-$offs] > ($size + $er)
          && $pradar[170-$offs] > ($size + $er)
          && $pradar[190-$offs] > ($size + $er)
          && $pradar[165-$offs] > ($size + $er)
          && $pradar[195-$offs] > ($size + $er)
          && $pradar[160-$offs] > ($size + $er)
          && $pradar[200-$offs] > ($size + $er)
          && $pradar[140-$offs] > ($size + $er)
          && $pradar[220-$offs] > ($size + $er)
          && $pradar[120-$offs] > ($size + $er)
          && $pradar[240-$offs] > ($size + $er)
         ) {

         Move;
         Move;
         $n += 2;

      } else {

         last;              
      }
      Killed;
   }
   Trace("Go2Wall: moved $n steps.");

   return $n;
}

# turn so that the nearest wall is exactly on the right side
sub WallOnRight
{
   my $dist = 1000.0;
   my $angle = 0.0;
   my $i = 0;

   # look for shortest radar reading index on the right,
   # 90 deg angle to the nearest wall
   if ($rrr == 0) {
      for ($i = 0; $i < 180; $i++) {
         if ($radar[$i] < $dist) {
            $dist = $radar[$i];
            $angle = - (90 - $i) * $pi / 180;
         }
      }
   } else {
      @pradar = PartRadar(0, 180);
      for ($i = 0; $i < 180; $i++) {
         if ($pradar[$i] < $dist) {
            $dist = $pradar[$i];
            $angle = - (90 - $i) * $pi / 180;
         }
      }
   }
   Turn $angle;
   Trace("WallOnRight: Turn $angle");
}

# make next step and position in such a way that the wall is always to the right
sub FollowWall
{
   Trace("FollowWall - start");
   my $d90 = $size + $er;
   my $n = 0;
   my $t = 0;
   my $correctevery = 10;
   my $a90 = $pi / 2 + $pi / 8;
   Killed;
   while ($iskilled eq 'false') {

      my $offs = 120;
      @pradar = PartRadar($offs, 121);
      # look carefully before moving
      if ($pradar[180-$offs] > ($size + $er) 
          && $pradar[175-$offs] > ($size + $er) 
          && $pradar[185-$offs] > ($size + $er)
          && $pradar[170-$offs] > ($size + $er)
          && $pradar[190-$offs] > ($size + $er)
          && $pradar[165-$offs] > ($size + $er)
          && $pradar[195-$offs] > ($size + $er)
          && $pradar[160-$offs] > ($size + $er)
          && $pradar[200-$offs] > ($size + $er)
          && $pradar[140-$offs] > ($size + $er)
          && $pradar[220-$offs] > ($size + $er)
          && $pradar[120-$offs] > ($size + $er)
          && $pradar[240-$offs] > ($size + $er)
         ) {

         Move;
         Move;
         $n += 2;
         Killed;
         if ($iskilled eq 'false') {
            if ($n%$correctevery == 0) {
               @pradar = PartRadar(90, 2);
               Trace("FollowWall: correctevery=$correctevery");
               my $corrangle = 0.02;
               if ($pradar[0] > $d90) {
                  if ($pradar[0] > $d90 * 1.5) {
                     Trace("FollowWall - correction, right");
                     $corrangle = - $corrangle * $pradar[0];
                     Turn $corrangle;
                     $t++;
                     Move;
                     WallOnRight;
                     Move;
                     $n += 2;
                  } else {
                     Trace("FollowWall - correction, right");
                     $corrangle = - $corrangle * $pradar[0];
                  }
               } else {
                  Trace("FollowWall - correction, left");
                  $corrangle = $corrangle * $pradar[0];
               }
               Turn $corrangle;
               $t++;
            }
         }
      } else {
         Trace("FollowWall: wall ahead too close to proceed.");
         WallOnRight;
         $offs = 90;
         @pradar = PartRadar($offs, 92);
         my @aheadrays = ($pradar[179-$offs], $pradar[180-$offs], $pradar[181-$offs]);
         $steps2go = int(min(@aheadrays) / $step - ($size / $step) * 3);
         if ($steps2go <= 0) {
            Trace("FollowWall: Turn $a90");
            Turn $a90;
            $t++;
            WallOnRight;
         } else {
                 Trace("FollowWall: moving ahead $steps2go steps.");
                 while ($steps2go > 0 && $iskilled eq 'false'
                         && (min(@aheadrays) / $step - ($size / $step) * 3) < ($size + $er) 
                         && $pradar[90-$offs] <= $d90 * 1.5) {
                    Move;
                    Move;
                    @pradar = PartRadar($offs, 92);
                    $n += 2;
                    $steps2go -= 2;
                    Killed;
                    $iskilled eq 'true' && last;
                    if ($n%$correctevery == 0) {
                       Trace("FollowWall: correctevery=$correctevery");
                       if ($pradar[90-$offs] > $d90) {
                          if ($pradar[90-$offs] > $d90 * 1.5) {
                             WallOnRight;
                             Move;
                             Move;
                             Killed;
                             $n += 2;
                             @pradar = PartRadar($offs, 92);
                          } 
                       }
                    }
                    @aheadrays = ($pradar[179-$offs], $pradar[180-$offs], $pradar[181-$offs]);
                 }
                 last;
         }
      }
   }
   Trace("FollowWall: walked $n steps, turned $t times.");

   return $n;
}

# Disable buffering
$|=1;

$size = GetSize ();
$step = GetStep ();

$minsteps = 1;
 
$er = $step * $minsteps;  # proximity to wall threshold
Trace("er=$er");

ReadRadar;

$stpstaken = 0;
$prevsteps = 0;

$stpstaken = Go2Wall;
$prevsteps = $stpstaken;
Killed;
while ($iskilled eq 'false')
{
   WallOnRight;
   Killed;
   if ($iskilled eq 'false') {
      $stpstaken = $stpstaken + FollowWall;
   }
   Killed;
   if ($iskilled eq 'false' && $stpstaken == $prevsteps) {
      my $angle = 0.0;
      my $offs = 90;
      my $a90 = $pi / 2 + $pi / 8;
      Trace("MAIN: Robot is stuck, try to get it out of the bind.");
      @pradar = PartRadar($offs, 92);
      my @aheadrays = ($pradar[179-$offs], $pradar[180-$offs], $pradar[181-$offs]);
      $steps2go = int(min(@aheadrays) / $step - ($size / $step) * 3);
      if ($steps2go <= 0) {
         Trace("Turn $a90");
         Turn $a90;
         WallOnRight;
      } else {
         while ($iskilled eq 'false' && $steps2go > 0) {
            Move;
            $steps2go--;
            Killed;
         }
      }
   }
   $prevsteps = $stpstaken;
   sleep(0.1);
};
