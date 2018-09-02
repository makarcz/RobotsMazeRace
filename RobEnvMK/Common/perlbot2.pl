#!/usr/bin/perl

sub Move
{
   print "Move\n";
   sleep (0.1);
}
 
sub Turn
{
   print "Turn $_[0]\n";
   sleep (0.1);
}

sub GetSize
{
   print "?Size\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}
 
sub GetStep
{
   print "?Step\n";
   sleep(0.1);
   chomp($resp = <STDIN>);
   sleep(0.1);

   return $resp;
}

sub Killed
{
   print "?Killed\n";
   sleep (0.1);
   chomp($resp = <STDIN>);
   sleep (0.1);
 
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

# Disable buffering
$|=1;

$size = GetSize ();
$step = GetStep ();
 
$pi = 3.14159265359;
$tl = $pi/2;
$tr = -$tl;
$er = $step*3;
$op = $step*10;

Turn $tl;
$iskilled = Killed ();
while ($iskilled eq 'false')
{
   @radar = Radar ();

   $lf = $radar[180];
   $lr = $radar[90];
   $ll = $radar[270];

   if ($lr > $lf && $lr > $op)
   {
      Turn $tr;
   }
   else
   {
      if ($ll > $lf && $ll > $op)
      {
         Turn $tl;
      }
      else

      {
   
   if ($lf < ($size + $er))
   {

      if ($lr > ($size + $er))
      {
         if ($lr > $ll)
	 {
            Turn $tr;
	 }
	 else
	 {
	    if ($ll > ($size + $er))
	    {
	       Turn $tl;
	    }
	    else
	    {
	       Turn $pi;
	    }
	 }
      }
      else
      {
	  if ($ll > ($size + $er))
	  {
	     if ($ll > $lr)
	     {
	        Turn $tl;
	     }
	     else
	     {
	        if ($lr > ($size + $er))
	        {
	           Turn $tr;
	        }
	        else
	        {
	           Turn $pi;
	        }
	     }
	  }
	  else
	  {
	     Turn 0.1;
	  }
      }
   }
   else
   {
      Move;
   }

   }

   }

   $iskilled = Killed ();
   sleep(0.1);
};
