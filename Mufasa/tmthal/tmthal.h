#ifdef TMTHAL_EXPORTS
#define TMTHAL_API __declspec(dllexport) 
#else
#define TMTHAL_API __declspec(dllimport) 
#endif
