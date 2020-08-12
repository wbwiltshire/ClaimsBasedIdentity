using System;
using ClaimsBasedIdentity.Data.Interfaces;

namespace ClaimsBasedIdentity.Data
{
    public class PrimaryKey : IPrimaryKey
    {
        private string tempString = String.Empty;
        public object Key { get; set; }
        public object[] CompositeKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComposite { get; set; }

        //ctor
        public PrimaryKey()
        {
            IsIdentity = false;
            IsComposite = false;
        }

        public override string ToString()
        {
            if (IsComposite)
            {
                foreach (object k in CompositeKey)
                    tempString += k.ToString() + "|";
                return $"|{tempString}{IsComposite}";
            }
            else
                return $"|{Key}|{IsIdentity}";
        }
    }
}
