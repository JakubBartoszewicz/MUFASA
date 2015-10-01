
#include "thal.c"
#include "oligotm.c"

/* Structure for passing arguments to THermodynamic ALignment calculation */
extern "C"  __declspec(dllexport) typedef struct {
	int debug; /* if non zero, print debugging info to stderr */
	thal_alignment_type type; /* one of the
							  1 THAL_ANY, (by default)
							  2 THAL_END1,
							  3 THAL_END2,
							  4 THAL_HAIRPIN */
	int maxLoop;  /* maximum size of loop to consider; longer than 30 bp are not allowed */
	double mv; /* concentration of monovalent cations */
	double dv; /* concentration of divalent cations */
	double dntp; /* concentration of dNTP-s */
	double dna_conc; /* concentration of oligonucleotides */
	double temp; /* temperature from which hairpin structures will be calculated */
	int temponly; /* if non zero, print only temperature to stderr */
	int dimer; /* if non zero, dimer structure is calculated */
} p3_thal_args;


/* Structure for receiving results from the thermodynamic alignment calculation */
extern "C"  __declspec(dllexport) typedef struct {
	char msg[255];
	double temp;
	int align_end_1;
	int align_end_2;
} p3_thal_results;



/* Central method for finding the best alignment.  On error, o->temp
is set to THAL_ERROR_SCORE and a message is put in o->msg.  The
error might be caused by ENOMEM. To determine this it is necessary
to check errno.
*/
extern "C"  __declspec(dllexport) void p3_thal(const unsigned char *oligo_f,
	const unsigned char *oligo_r,
	const p3_thal_args *a,
	p3_thal_results *o)
{
	thal_args *_a = new thal_args;
	_a->debug = a->debug;
	_a->type = a->type;
	_a->maxLoop = a->maxLoop;
	_a->mv = a->mv;
	_a->dv = a->dv;
	_a->dntp = a->dntp;
	_a->dna_conc = a->dna_conc;
	_a->temp = a->temp;
	_a->temponly = a->temponly;
	_a->dimer = a->dimer;

	thal_results * _o = new thal_results;
	_o->align_end_1 = o->align_end_1;
	_o->align_end_2 = o->align_end_2;
	strcpy(_o->msg, o->msg);
	_o->temp = o->temp;

	thal(oligo_f, oligo_r, _a, _o);
}




/* Return the melting temperature of a given sequence, 'seq', of any
length.

If tm_method==santalucia_auto, then the table of
nearest-neighbor thermodynamic parameters and method for Tm
calculation in the paper [SantaLucia JR (1998) "A unified view of
polymer, dumbbell and oligonucleotide DNA nearest-neighbor
thermodynamics", Proc Natl Acad Sci 95:1460-65
http://dx.doi.org/10.1073/pnas.95.4.1460] is used.
*THIS IS THE RECOMMENDED VALUE*.

If tm_method==breslauer_auto, then method for Tm
calculations in the paper [Rychlik W, Spencer WJ and Rhoads RE
(1990) "Optimization of the annealing temperature for DNA
amplification in vitro", Nucleic Acids Res 18:6409-12
http://www.pubmedcentral.nih.gov/articlerender.fcgi?tool=pubmed&pubmedid=2243783].
and the thermodynamic parameters in the paper [Breslauer KJ, Frank
R, Blöcker H and Marky LA (1986) "Predicting DNA duplex stability
from the base sequence" Proc Natl Acad Sci 83:4746-50
http://dx.doi.org/10.1073/pnas.83.11.3746], are is used.  This is
the method and the table that primer3 used up to and including
version 1.0.1

If salt_corrections==schildkraut, then formula for
salt correction in the paper [Schildkraut, C, and Lifson, S (1965)
"Dependence of the melting temperature of DNA on salt
concentration", Biopolymers 3:195-208 (not available on-line)] is
used.  This is the formula that primer3 used up to and including
version 1.0.1.

If salt_corrections==santalucia, then formula for
salt correction suggested by the paper [SantaLucia JR (1998) "A
unified view of polymer, dumbbell and oligonucleotide DNA
nearest-neighbor thermodynamics", Proc Natl Acad Sci 95:1460-65
http://dx.doi.org/10.1073/pnas.95.4.1460] is used.

*THIS IS THE RECOMMENDED VALUE*.

If salt_corrections==owczarzy, then formula for
salt correction in the paper [Owczarzy, R., Moreira, B.G., You, Y.,
Behlke, M.A., and Walder, J.A. (2008) "Predicting stability of DNA
duplexes in solutions containing magnesium and monovalent cations",
Biochemistry 47:5336-53 http://dx.doi.org/10.1021/bi702363u] is used.

*/
extern "C"  __declspec(dllexport) double p3_seqtm(const char* seq,  /* The sequence. */
	double dna_conc,   /* DNA concentration (nanomolar). */
	double salt_conc,  /* Concentration of divalent cations (millimolar). */
	double divalent_conc, /* Concentration of divalent cations (millimolar) */
	double dntp_conc,     /* Concentration of dNTPs (millimolar) */
	int    nn_max_len,  /* The maximum sequence length for
						using the nearest neighbor model
						(as implemented in oligotm.  For
						sequences longer than this, seqtm
						uses the "GC%" formula implemented
						in long_seq_tm.
						*/

						tm_method_type  tm_method,       /* See description above. */
						salt_correction_type salt_corrections /* See description above. */
						)
{
	return seqtm(seq, dna_conc, salt_conc, divalent_conc, dntp_conc, nn_max_len, tm_method, salt_corrections);
}