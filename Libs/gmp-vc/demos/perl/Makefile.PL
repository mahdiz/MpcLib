# Makefile for GMP perl module.

# Copyright 2001 Free Software Foundation, Inc.
#
# This file is part of the GNU MP Library.
#
# The GNU MP Library is free software; you can redistribute it and/or modify
# it under the terms of the GNU Lesser General Public License as published
# by the Free Software Foundation; either version 2.1 of the License, or (at
# your option) any later version.
#
# The GNU MP Library is distributed in the hope that it will be useful, but
# WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
# or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
# License for more details.
#
# You should have received a copy of the GNU Lesser General Public License
# along with the GNU MP Library; see the file COPYING.LIB.  If not, write to
# the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston,
# MA 02111-1307, USA.


# Bugs:
#
# When the generated Makefile re-runs "perl Makefile.PL" the GMP_BUILDDIR
# parameter is lost.


use ExtUtils::MakeMaker;


# Find and remove our parameters
@ARGV = map {
  if (/^GMP_BUILDDIR=(.*)/) {
    $GMP_BUILDDIR=$1; ();
  } else {
    $_;
  }
} (@ARGV);

$INC = "";
$LIBS = "-lgmp";
$OBJECT = "GMP.o";

if (defined $GMP_BUILDDIR) {
  if (! -f "$GMP_BUILDDIR/libgmp.la") {
    die "$GMP_BUILDDIR doesn't contain libgmp.la\n" .
	"if it's really a gmp build directory then go there and run \"make libgmp.la\"\n";
  }
  $INC = "-I$GMP_BUILDDIR $INC";
  $LIBS = "-L$GMP_BUILDDIR/.libs $LIBS";
}

WriteMakefile(
	      NAME         => 'GMP',
	      VERSION      => '1',
	      LIBS         => [$LIBS],
	      OBJECT       => $OBJECT,
	      INC          => $INC,
	      clean        => { FILES => 'test.tmp' },
	      );


# Local variables:
# perl-indent-level: 2
# End:
