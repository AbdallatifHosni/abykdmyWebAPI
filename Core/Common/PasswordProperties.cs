using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public class PasswordProperties
    {
        public PasswordProperties(int length,bool includeSpecialChars , bool includeNumbers , bool includeLowerCase , bool includeUpperCase )
        {
            Length=length;
            IncludeSpecialChars=includeSpecialChars;    
            IncludeNumbers=includeNumbers;  
            IncludeLowerCase=includeLowerCase;  
            IncludeUpperCase=includeUpperCase;  
        }
        public int Length { get; set; }
        public bool IncludeSpecialChars { get; set; }
        public bool IncludeNumbers { get; set; }
        public bool IncludeLowerCase { get; set; }
        public bool IncludeUpperCase { get; set; }
    }
}
