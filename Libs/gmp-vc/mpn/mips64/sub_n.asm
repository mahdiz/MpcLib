dnl  MIPS64 mpn_sub_n -- Subtract two limb vectors of the same length > 0 and
dnl  store difference in a third limb vector.

dnl  Copyright 1995, 2000, 2001, 2002 Free Software Foundation, Inc.

dnl  This file is part of the GNU MP Library.

dnl  The GNU MP Library is free software; you can redistribute it and/or modify
dnl  it under the terms of the GNU Lesser General Public License as published
dnl  by the Free Software Foundation; either version 2.1 of the License, or (at
dnl  your option) any later version.

dnl  The GNU MP Library is distributed in the hope that it will be useful, but
dnl  WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
dnl  or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
dnl  License for more details.

dnl  You should have received a copy of the GNU Lesser General Public License
dnl  along with the GNU MP Library; see the file COPYING.LIB.  If not, write to
dnl  the Free Software Foundation, Inc., 59 Temple Place - Suite 330, Boston,
dnl  MA 02111-1307, USA.

include(`../config.m4')

C INPUT PARAMETERS
C res_ptr	$4
C s1_ptr	$5
C s2_ptr	$6
C size		$7

ASM_START()
PROLOGUE(mpn_sub_n)
	ld	$10,0($5)
	ld	$11,0($6)

	daddiu	$7,$7,-1
	and	$9,$7,4-1	C number of limbs in first loop
	beq	$9,$0,.L0	C if multiple of 4 limbs, skip first loop
	 move	$2,$0

	dsubu	$7,$7,$9

.Loop0:	daddiu	$9,$9,-1
	ld	$12,8($5)
	daddu	$11,$11,$2
	ld	$13,8($6)
	sltu	$8,$11,$2
	dsubu	$11,$10,$11
	sltu	$2,$10,$11
	sd	$11,0($4)
	or	$2,$2,$8

	daddiu	$5,$5,8
	daddiu	$6,$6,8
	move	$10,$12
	move	$11,$13
	bne	$9,$0,.Loop0
	 daddiu	$4,$4,8

.L0:	beq	$7,$0,.Lend
	 nop

.Loop:	daddiu	$7,$7,-4

	ld	$12,8($5)
	dsubu	$11,$10,$11
	ld	$13,8($6)
	sltu	$8,$10,$11
	dsubu	$14,$11,$2
	sltu	$2,$11,$14
	sd	$14,0($4)
	or	$2,$2,$8

	ld	$10,16($5)
	dsubu	$13,$12,$13
	ld	$11,16($6)
	sltu	$8,$12,$13
	dsubu	$14,$13,$2
	sltu	$2,$13,$14
	sd	$14,8($4)
	or	$2,$2,$8

	ld	$12,24($5)
	dsubu	$11,$10,$11
	ld	$13,24($6)
	sltu	$8,$10,$11
	dsubu	$14,$11,$2
	sltu	$2,$11,$14
	sd	$14,16($4)
	or	$2,$2,$8

	ld	$10,32($5)
	dsubu	$13,$12,$13
	ld	$11,32($6)
	sltu	$8,$12,$13
	dsubu	$14,$13,$2
	sltu	$2,$13,$14
	sd	$14,24($4)
	or	$2,$2,$8

	daddiu	$5,$5,32
	daddiu	$6,$6,32

	bne	$7,$0,.Loop
	 daddiu	$4,$4,32

.Lend:	daddu	$11,$11,$2
	sltu	$8,$11,$2
	dsubu	$11,$10,$11
	sltu	$2,$10,$11
	sd	$11,0($4)
	j	$31
	or	$2,$2,$8
EPILOGUE(mpn_sub_n)
