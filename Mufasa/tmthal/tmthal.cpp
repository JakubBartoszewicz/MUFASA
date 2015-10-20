
#include "tmthal.h"
#include "thal.c"
#include "oligotm.c"


namespace tmthal
{
	/* Central method for finding the best alignment.  On error, o->temp
	is set to THAL_ERROR_SCORE and a message is put in o->msg.  The
	error might be caused by ENOMEM. To determine this it is necessary
	to check errno.
	*/
	extern "C" TMTHAL_API void p3_thal(const unsigned char *oligo_f,
		const unsigned char *oligo_r,
		const thal_args *a,
		thal_results *o)
	{		
		//thal_results *o = new thal_results;
		get_thermodynamic_values(".\\primer3_config\\", o);
		thal(oligo_f, oligo_r, a, o);
		//return *o;
	}

	/* Read the thermodynamic values (parameters) from the parameter files
	in the directory specified by 'path'.  Return 0 on success and -1
	on error. The thermodynamic values are stored in multiple static
	variables. */
	//extern "C" TMTHAL_API thal_results p3_get_thermodynamic_values(const char* path)
	//{
	//	thal_results *o = new thal_results;
	//	get_thermodynamic_values(path, o);
	//	return *o;
	//}


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
	extern "C" TMTHAL_API double p3_seqtm(const char* seq,  /* The sequence. */
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
}