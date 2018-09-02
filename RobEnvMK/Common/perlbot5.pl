#!/usr/bin/perl
# This bot follows the wall to the right.
#
use List::Util qw(min max);

my @radar;
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

   open(TRCHANDLE,">>perlbot5.trc") or die;
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

   for ($i = 0; $i < 180; $i++) {
      if ($radar[$i] < $dist) {
         $dist = $radar[$i];
         $angle = - (180 - $i) * $pi / 180;
      }
   }
   Trace("Go2Wall: Turn to closest wall to the right, angle = $angle");
   Turn $angle;
   ReadRadar;
   Killed;
   while ($iskilled eq 'false')
   {
      ReadRadar;
      # look carefully before moving
      if ($radar[180] > ($size + $er) 
          && $radar[175] > ($size + $er) 
          && $radar[185] > ($size + $er)
          && $radar[170] > ($size + $er)
          && $radar[190] > ($size + $er)
          && $radar[165] > ($size + $er)
          && $radar[195] > ($size + $er)
          && $radar[160] > ($size + $er)
          && $radar[200] > ($size + $er)
          && $radar[140] > ($size + $er)
          && $radar[220] > ($size + $er)
          && $radar[120] > ($size + $er)
          && $radar[240] > ($size + $er)
         ) {

         Move;
         $n = $n + 1;

      } else {

         last;              
      }
      Killed;
   }
   ReadRadar;
   Trace("Go2Wall: moved $n steps.");

   return $n;
}

# turn so that the nearest wall is exactly on the right side
sub WallOnRight
{
   my $dist = 1000.0;
   my $angle = 0.0;
   my $i = 0;

   ReadRadar;
   #Trace("WallOnRight: DBG - dumping radar before turn.");
   #DumpRadar;
   # look for shortest radar reading index on the right,
   # 90 deg angle to the nearest wall
   for ($i = 0; $i < 180; $i++) {
      if ($radar[$i] < $dist) {
         $dist = $radar[$i];
         $angle = - (90 - $i) * $pi / 180;
         #Trace("WallOnRight: searching shortest distance, found: deg=$i, dist=$dist, angle=$angle");
      }
   }
   Turn $angle;
   Trace("WallOnRight: Turn $angle");
   ReadRadar;
   #Trace("WallOnRight: DBG - dumping radar after turn.");
   #DumpRadar;
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

      ReadRadar;

      if ($radar[180] > ($size + $er)
          && $radar[175] > ($size + $er) 
          && $radar[185] > ($size + $er)
          && $radar[170] > ($size + $er)
          && $radar[190] > ($size + $er)
          && $radar[165] > ($size + $er)
          && $radar[195] > ($size + $er)
          && $radar[160] > ($size + $er)
          && $radar[200] > ($size + $er)
          && $radar[140] > ($size + $er)
          && $radar[220] > ($size + $er)
          && $radar[120] > ($size + $er)
          && $radar[240] > ($size + $er)
         ) {

         Move;
         $n = $n + 1;
         Killed;
         if ($iskilled eq 'false') {
            ReadRadar;
            if ($n%$correctevery == 0) {
               Trace("FollowWall: correctevery=$correctevery");
               my $corrangle = 0.02;
               if ($radar[90] > $d90) {
                  if ($radar[90] > $d90 * 1.5) {
                     WallOnRight;
                     Move;
                     $n++;
                  } else {
                     Trace("FollowWall - correction, right");
                     $corrangle = - $corrangle * $radar[90];
                  }
               } else {
                  Trace("FollowWall - correction, left");
                  $corrangle = $corrangle * $radar[90];
               }
               Turn $corrangle;
               $t = $t + 1;
            }
         }
      } else {
         Trace("FollowWall: wall ahead too close to proceed.");
         WallOnRight;
         ReadRadar;
         my @aheadrays = ($radar[179], $radar[180], $radar[181]);
         $steps2go = int(min(@aheadrays) / $step - ($size / $step) * 3);
         if ($steps2go <= 0) {
            Trace("FollowWall: Turn $a90");
            Turn $a90;
            $t++;
            ReadRadar;
            WallOnRight;
         } else {
                 Trace("FollowWall: moving ahead $steps2go steps.");
                 while ($steps2go > 0 && $iskilled eq 'false'
                         && (min(@aheadrays) / $step - ($size / $step) * 3) < ($size + $er) 
                         && $radar[90] <= $d90 * 1.5) {
                    Move;
                    ReadRadar;
                    $n = $n + 1;
                    $steps2go--;
                    Killed;
                    $iskilled eq 'true' && last;
                    if ($n%$correctevery == 0) {
                       Trace("FollowWall: correctevery=$correctevery");
                       if ($radar[90] > $d90) {
                          if ($radar[90] > $d90 * 1.5) {
                             WallOnRight;
                             Move;
                             $n++;
                          } 
                       }
                    }
                    ReadRadar;
                    @aheadrays = ($radar[179], $radar[180], $radar[181]);
                 }
                 last;
         }
      }
   }
   ReadRadar;
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
   ReadRadar ();

   WallOnRight;
   Killed;
   if ($iskilled eq 'false') {
      $stpstaken = $stpstaken + FollowWall;
   }
   Killed;
   if ($iskilled eq 'false' && $stpstaken == $prevsteps) {
      my $angle = 0.0;
      ReadRadar;
      Trace("MAIN: Robot is stuck, try to get it out of the bind.");
      if ($radar[170] < $radar[190]) {
         $angle = 10 * $pi / 180;
      } else {
         $angle = -10 * $pi / 180;
      }
      my $steps2go = 10;
      Trace("MAIN: Attempt to unstuck robot - Turn $angle, $steps2go x Move");
      Turn $angle;
      Killed;
      while ($iskilled eq 'false' && $steps2go > 0) {
         Move;
         $steps2go--;
         Killed;
      }
   }
   $prevsteps = $stpstaken;
   sleep(0.1);
};
