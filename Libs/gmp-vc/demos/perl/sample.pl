#!/usr/bin/perl -w

# Some sample GMP module operations

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

use strict;


use GMP::Mpz qw(:all);
print "the 200th fibonacci number is ", fib(200), "\n";
print "next prime after 10**30 is (probably) ", nextprime(mpz(10)**30), "\n";


use GMP::Mpq qw(:constants);
print "the 7th harmonic number is ", 1+1/2+1/3+1/4+1/5+1/6+1/7, "\n";
use GMP::Mpq qw(:noconstants);


use GMP::Mpf qw(mpf);
my $f = mpf(1,180);
$f >>= 180;
$f += 1;
print "a sample mpf is $f\n";
