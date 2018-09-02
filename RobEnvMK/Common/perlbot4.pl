#!/usr/bin/perl

my $pi = 3.1415926535897932384626433832795;
my $tl = $pi/2;  # angle to turn left
my $tr = -$tl;   # angle to turn right
my $sslm = 0; # steps since last Move
my $rnd = rand(10);
my $TRCHANDLE = "trchandle";
my $rrad = 1;
my @radval;

sub Move
{
   print "Move\n";
   $sslm = 0;
   $rrad = 1;
   sleep (0.1);
   Trace ("Move");
}
 
sub Turn
{
   print "Turn $_[0]\n";
   $rrad = 1;
   sleep (0.1);
   Trace ("Turn $_[0]");
}

sub GetSize
{
   my $resp;
   print "?Size\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}
 
sub GetStep
{
   my $resp;
   print "?Step\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}

sub Killed
{
   my $resp;
   print "?Killed\n";
   sleep (0.1);
   chomp($resp = <STDIN>);
   sleep (0.1);
 
   return $resp;
}

sub Radar
{
   print "?Radar\n";
   $rrad = 0;
   sleep (0.1);
   chomp($data = <STDIN>);
   sleep (0.1);
   @radval = split (/ /, $data);

   return @radval;
}

sub GetPossibleMoves
{
   my($treshold,@radar) = @_;
   my @ret;
   my $rv6 = "";
   $i = 0;
      
   Trace ("GetPossibleMoves---");
   for $it (@radar)
   {
      my $range = 100;
      if ($it > $treshold && $i > 180-$range && $i < 180+$range)
	  {
         my $add = 1;
		 $rv6 = "";
		 my $sweep = 7;
         for (my $n = $i-$sweep; $n <= $i+$sweep; $n++)
	     {
		    $rv6 = $rv6 . "\n$radar[$n]";
	        if ($radar[$n] < $it)
		    {
		       $add = 0;
			   last;
		    }
	     }
		 if ($add == 1)
		 {
	        push(@ret,$i);
			Trace ("Added index: $i, range: $it");
			Trace ("Radar values: $rv6");
		 }
	  }
	  $i++;
   }

   return @ret;
   
}

sub NormalizeRadarIndex
{
   my $idx = $_[0];
   if ($idx < 0)
   {
      $idx = $idx + 360;
   }
   if ($idx > 359)
   {
      $idx = $idx - 360;
   }

   return $idx;
}

sub GetTurnAngle
{
   my $idx = $_[0] - 180;
   my $angle = ($pi/180) * $idx;

   return $angle;
}

sub Correct
{
   my $treshold = $_[0];
   my $ret = 0;
   my @radar = $radval;
   if ($rrad == 1)
   {
      @radar = Radar();
   }
#   my $rndv = rand(10);
   my $n = 0;

   my $sign = ($rnd > 5) ? -1 : 1;
   
   for (my $i = 180; $n < 40; $i = $i + $sign)
   {
      if ($radar[$i] < $treshold)
      {
         Turn ($sign*$tr/6);
         $ret = 1;
         last;
      }
	  $n++;
   }

   if ($ret == 0)
   {
      $n = 0;
	  $sign = -$sign;
      for (my $i = 180; $n < 40; $i = $i + $sign)
      {
         if ($radar[$i] < $treshold)
         {
            Turn ($sign*$tr/6);
            $ret = 2;
            last;
         }
	     $n++;
      }
   }

   Trace ("Correct, ret=$ret");

   return $ret;
}

sub Trace
{
   my $text = $_[0];

   open(TRCHANDLE,">>bot4.trc") or die;
   printf TRCHANDLE "$text\n";
   close TRCHANDLE
}

# Disable buffering
$|=1;

my $size = GetSize ();
my $step = GetStep ();

 
my $corrsteps = 1000000;

my $iskilled = Killed ();
Trace ("perlbot4 started");
while ($iskilled eq 'false')
{
   my $csslm = 0;
   my @radar = $radval;
   if ($rrad == 1)
   {
      @radar = Radar();
   }
   my @moves = GetPossibleMoves($size*3.4,@radar);
   my $prev = -1;
   my $turn = 0;
   my $index = -1;
   for $item (@moves)
   {
      if ($radar[$item] > $prev)
	  {
	     $index = $item;
		 $prev = $radar[$index];
	  }
   }
   if ($index >= 0)
   {
      Trace ("Found possible moves.");
      $index = NormalizeRadarIndex($index);
      $turn = GetTurnAngle($index);
      Trace ("index=$index");
      Trace ("turn=$turn");
      if ($turn != 0)
      {
         Turn $turn;
      }
      $numofsteps = ($radar[$index] - ($size*1.4))/$step - 2;
	  Trace ("numofsteps=$numofsteps");
      for (my $n = 0; $n < $numofsteps; $n++)
      {
         if ($n > 0 && ($n % $corrsteps) == 0)
	     {
		    my $crsteps=6;
	        while(Correct($size*1.5) != 0 && $crsteps > 0)
			{
				$crsteps--;
			}
		    if ($crsteps < 6)
		    {
		       last;
		    }
	     }
         Move;
         $iskilled = Killed();
	     if ($iskilled eq 'true')
	     {
		    Trace("bot killed");
	        last;
	     }
      }
   }

   if (($sslm % 50) == 0)
   {
      $rnd = rand(10);
   }

   if ($sslm > 100)
   {
      my $trn = 22;
	  my $sign = ($rnd > 5) ? 1 : -1;
      if ($rrad == 1)
	  {
	     @radar = Radar();
	  }
	  if ($radar[180-$trn] > $radar[180+$trn])
	  {
	     $sign = -1;
      }
	  else 
	  {
	     if ($radar[180-$trn] < $radar[180+$trn])
		 {
	        $sign = 1;
		 }
	  }
      Turn($sign*GetTurnAngle(180+$trn));
	  $csslm = 1;
      Trace("sslm correction");
   }
   else
   {
      if ($csslm != 0)
	  {
         $csslm = 0;
         $rnd = rand(10);
	  }
   }

   $sslm++;
   
   if ($csslm == 0) # drift
   {
	  my $sign = ($rnd > 5) ? 1 : -1;
      Turn($sign*GetTurnAngle(180+5));
   }
   sleep(0.1);
};

close (TRCHANDLE);

