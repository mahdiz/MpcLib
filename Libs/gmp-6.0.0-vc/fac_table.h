/* This file is automatically generated by gen-fac.c */

//#if GMP_NUMB_BITS != 32
//Error , error this data is for 32 GMP_NUMB_BITS only
//#endif
/* This table is 0!,1!,2!,3!,...,n! where n! has <= GMP_NUMB_BITS bits */
#define ONE_LIMB_FACTORIAL_TABLE CNST_LIMB(0x1),CNST_LIMB(0x1),CNST_LIMB(0x2),CNST_LIMB(0x6),CNST_LIMB(0x18),CNST_LIMB(0x78),CNST_LIMB(0x2d0),CNST_LIMB(0x13b0),CNST_LIMB(0x9d80),CNST_LIMB(0x58980),CNST_LIMB(0x375f00),CNST_LIMB(0x2611500),CNST_LIMB(0x1c8cfc00)

/* This table is 0!,1!,2!/2,3!/2,...,n!/2^sn where n!/2^sn is an */
/* odd integer for each n, and n!/2^sn has <= GMP_NUMB_BITS bits */
#define ONE_LIMB_ODD_FACTORIAL_TABLE CNST_LIMB(0x1),CNST_LIMB(0x1),CNST_LIMB(0x1),CNST_LIMB(0x3),CNST_LIMB(0x3),CNST_LIMB(0xf),CNST_LIMB(0x2d),CNST_LIMB(0x13b),CNST_LIMB(0x13b),CNST_LIMB(0xb13),CNST_LIMB(0x375f),CNST_LIMB(0x26115),CNST_LIMB(0x7233f),CNST_LIMB(0x5cca33),CNST_LIMB(0x2898765),CNST_LIMB(0x260eeeeb),CNST_LIMB(0x260eeeeb)
#define ODD_FACTORIAL_TABLE_MAX CNST_LIMB(0x260eeeeb)
#define ODD_FACTORIAL_TABLE_LIMIT (16)

/* Previous table, continued, values modulo 2^GMP_NUMB_BITS */
#define ONE_LIMB_ODD_FACTORIAL_EXTTABLE CNST_LIMB(0x86fddd9b),CNST_LIMB(0xbeecca73),CNST_LIMB(0x2b930689),CNST_LIMB(0xd9df20ad),CNST_LIMB(0xdf4dae31),CNST_LIMB(0x98567c1b),CNST_LIMB(0xafc5266d),CNST_LIMB(0xf4f7347),CNST_LIMB(0x7ec241ef),CNST_LIMB(0x6fdd5923),CNST_LIMB(0xcc5866b1),CNST_LIMB(0x966aced7),CNST_LIMB(0xa196e5b),CNST_LIMB(0x977d7755),CNST_LIMB(0x5831734b),CNST_LIMB(0x5831734b),CNST_LIMB(0x5e5fdcab),CNST_LIMB(0x445da75b)
#define ODD_FACTORIAL_EXTTABLE_LIMIT (34)

/* This table is 1!!,3!!,...,(2n+1)!! where (2n+1)!! has <= GMP_NUMB_BITS bits */
#define ONE_LIMB_ODD_DOUBLEFACTORIAL_TABLE CNST_LIMB(0x1),CNST_LIMB(0x3),CNST_LIMB(0xf),CNST_LIMB(0x69),CNST_LIMB(0x3b1),CNST_LIMB(0x289b),CNST_LIMB(0x20fdf),CNST_LIMB(0x1eee11),CNST_LIMB(0x20dcf21),CNST_LIMB(0x27065f73)
#define ODD_DOUBLEFACTORIAL_TABLE_MAX CNST_LIMB(0x27065f73)
#define ODD_DOUBLEFACTORIAL_TABLE_LIMIT (19)

/* This table x_1, x_2,... contains values s.t. x_n^n has <= GMP_NUMB_BITS bits */
#define NTH_ROOT_NUMB_MASK_TABLE (GMP_NUMB_MASK),CNST_LIMB(0xffff),CNST_LIMB(0x659),CNST_LIMB(0xff),CNST_LIMB(0x54),CNST_LIMB(0x28),CNST_LIMB(0x17),CNST_LIMB(0xf)

/* This table contains inverses of odd factorials, modulo 2^GMP_NUMB_BITS */

/* It begins with (2!/2)^-1=1 */
#define ONE_LIMB_ODD_FACTORIAL_INVERSES_TABLE CNST_LIMB(0x1),CNST_LIMB(0xaaaaaaab),CNST_LIMB(0xaaaaaaab),CNST_LIMB(0xeeeeeeef),CNST_LIMB(0xa4fa4fa5),CNST_LIMB(0xf2ff2ff3),CNST_LIMB(0xf2ff2ff3),CNST_LIMB(0x53e3771b),CNST_LIMB(0xdd93e49f),CNST_LIMB(0xfcdee63d),CNST_LIMB(0x544a4cbf),CNST_LIMB(0x7ca340fb),CNST_LIMB(0xa417526d),CNST_LIMB(0xd7bd49c3),CNST_LIMB(0xd7bd49c3),CNST_LIMB(0x85294093),CNST_LIMB(0xf259eabb),CNST_LIMB(0xd6dc4fb9),CNST_LIMB(0x915f4325),CNST_LIMB(0x131cead1),CNST_LIMB(0xea76fe13),CNST_LIMB(0x633cd365),CNST_LIMB(0x21144677),CNST_LIMB(0x200b0d0f),CNST_LIMB(0x8c4f9e8b),CNST_LIMB(0x21a42251),CNST_LIMB(0xe03c04e7),CNST_LIMB(0x600211d3),CNST_LIMB(0x4aaacdfd),CNST_LIMB(0x33f4fe63),CNST_LIMB(0x33f4fe63)

/* This table contains 2n-popc(2n) for small n */

/* It begins with 2-1=1 (n=1) */
#define TABLE_2N_MINUS_POPC_2N 1,3,4,7,8,10,11,15,16,18,19,22,23,25,26,31,32,34,35,38,39,41,42,46
#define TABLE_LIMIT_2N_MINUS_POPC_2N 49
#define ODD_CENTRAL_BINOMIAL_OFFSET (8)

/* This table contains binomial(2k,k)/2^t */

/* It begins with ODD_CENTRAL_BINOMIAL_TABLE_MIN */
#define ONE_LIMB_ODD_CENTRAL_BINOMIAL_TABLE CNST_LIMB(0x1923),CNST_LIMB(0x2f7b),CNST_LIMB(0xb46d),CNST_LIMB(0x15873),CNST_LIMB(0xa50c7),CNST_LIMB(0x13d66b),CNST_LIMB(0x4c842f),CNST_LIMB(0x93ee7d),CNST_LIMB(0x11e9e123),CNST_LIMB(0x22c60053),CNST_LIMB(0x873ae4d1)
#define ODD_CENTRAL_BINOMIAL_TABLE_LIMIT (18)

/* This table contains the inverses of elements in the previous table. */
#define ONE_LIMB_ODD_CENTRAL_BINOMIAL_INVERSE_TABLE CNST_LIMB(0x16a2de8b),CNST_LIMB(0x847457b3),CNST_LIMB(0xfa6f7565),CNST_LIMB(0xf0e50cbb),CNST_LIMB(0xdca370f7),CNST_LIMB(0x9bb12643),CNST_LIMB(0xdc8342cf),CNST_LIMB(0x4ebf7ad5),CNST_LIMB(0x86ab568b),CNST_LIMB(0x265843db),CNST_LIMB(0x8633f431)

/* This table contains the values t in the formula binomial(2k,k)/2^t */
#define CENTRAL_BINOMIAL_2FAC_TABLE 1,2,2,3,2,3,3,4,1,2,2
