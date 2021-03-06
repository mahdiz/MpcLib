/* gmp_errno, __gmp_exception -- exception handling and reporting.

Copyright 2000, 2001 Free Software Foundation, Inc.

This file is part of the GNU MP Library.

The GNU MP Library is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2.1 of the License, or (at your
option) any later version.

The GNU MP Library is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
License for more details.

You should have received a copy of the GNU Lesser General Public License
along with the GNU MP Library; see the file COPYING.LIB.  If not, write to
the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston,
MA 02111-1307, USA. */

////#include <stdlib.h>
#include "gmp.h"
#include "gmp-impl.h"

int gmp_errno = 0;


/* The deliberate divide by zero triggers an exception on most systems.  On
   those where it doesn't, for example power and powerpc, use abort instead.

   Enhancement: Perhaps raise(SIGFPE) (or the same with kill()) would be
   better than abort.  Perhaps it'd be possible to get the BSD style
   FPE_INTDIV_TRAP parameter in there too.  */

void
__gmp_exception (int error_bit)
{
  gmp_errno |= error_bit;
  __gmp_junk = 10 / __gmp_0;
  abort ();
}


/* These functions minimize the amount of code required in functions raising
   exceptions.  Since they're "noreturn" and don't take any parameters, a
   test and call might even come out as a simple conditional jump.  */
void
__gmp_sqrt_of_negative (void)
{
  __gmp_exception (GMP_ERROR_SQRT_OF_NEGATIVE);
}
void
__gmp_divide_by_zero (void)
{
  __gmp_exception (GMP_ERROR_DIVISION_BY_ZERO);
}
