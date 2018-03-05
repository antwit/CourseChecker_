using NLog;
using System;
using System.Collections.Generic;

namespace CourseChecker.Course {

    internal class Kurse: IDisposable {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public String StrOrt { get; set; }
        public int IPreis { get; set; }
        public Boolean BoolGarantieTermin { get; set; }
        public DateTime DateEnde { get; set; }
        public DateTime DateBeginn { get; set; }
        public String StrKursTitel { get; set; }
        public String StrKursNr { get; set; }
        public String StrReason { get; set; }
        public String StrAnbieter { get; set; }
        public int IBuchungen { get; set; } = 0;
        public Uri Link { get; set; }
        public bool IsSelected { get; set;}

        private static readonly Dictionary<String, String> dictReason = new Dictionary<string, string>() {
            { "Preis", "Kurspreis wurde geändert;"},
            { "Garantie", "Kurs ist nun ein Garantietermin;" },
            { "!Garantie", "Kurs ist nicht mehr ein Garantietermin;" },
            { "!Kurs", "Kurs entfernt;" },
            { "Kurs", "Kurs hinzugefügt;" }
        };
        
        public Kurse(String strKursNr, String strKursTitel, DateTime dateBeginn, DateTime dateEnde, String strOrt, int iPreis, String strAnbieter, Uri link) {
            this.StrKursNr = strKursNr;
            this.StrKursTitel = strKursTitel;
            this.DateBeginn = dateBeginn;
            this.DateEnde = dateEnde;
            this.StrOrt = strOrt;
            this.IPreis = iPreis;
            this.BoolGarantieTermin = false;
            this.StrReason = "";
            this.StrAnbieter = strAnbieter;
            this.Link = link;
        }

        public Kurse(String strKursNr, String strKursTitel, DateTime dateBeginn, DateTime dateEnde, String strOrt, int iPreis, Boolean boolGarantieTermin, String strAnbieter, Uri link) {
            this.StrKursNr = strKursNr;
            this.StrKursTitel = strKursTitel;
            this.DateBeginn = dateBeginn;
            this.DateEnde = dateEnde;
            this.StrOrt = strOrt;
            this.IPreis = iPreis;
            this.StrReason = "";
            this.StrAnbieter = strAnbieter;
            this.BoolGarantieTermin = boolGarantieTermin;
            this.Link = link;
        }
        
        public Boolean Contains(Kurse kursCheck) {
            Boolean retEqual = false;
            Boolean boolCheckFields = this.StrKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.DateBeginn.Equals(kursCheck.DateBeginn)
                        & this.DateEnde.Equals(kursCheck.DateEnde)
                        & this.StrOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())
                        & this.IPreis.Equals(kursCheck.IPreis)
                        & this.BoolGarantieTermin.Equals(kursCheck.BoolGarantieTermin);

            if (this.StrKursNr.Length > 0)
                retEqual = boolCheckFields;
            else
                retEqual = boolCheckFields
                        & this.StrKursTitel.ToLowerInvariant()
                                .Replace("–", "-")
                                .Replace("/", "")
                                .Replace("ibm ", "")
                                .Replace(":", "").Trim()
                                .Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim());

            if(!retEqual && this.StrKursTitel.ToLowerInvariant()
                                    .Replace("–", "-")
                                    .Replace("/", "")
                                    .Replace("ibm ", "")
                                    .Replace(":", "").Trim()
                                    .Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim()) 
                                    && this.DateBeginn.Equals(kursCheck.DateBeginn) 
                                    && this.StrOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())) {

                if(!this.IPreis.Equals(kursCheck.IPreis))
                    this.StrReason = dictReason["Preis"];
                if(!this.BoolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && this.BoolGarantieTermin)
                    this.StrReason = dictReason["Garantie"];
                if(!this.BoolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && kursCheck.BoolGarantieTermin)
                    this.StrReason = dictReason["!Garantie"];
            } else if(!retEqual && !(this.StrReason.Length > 1)) {
                this.StrReason = dictReason["Kurs"];
            }
            return retEqual;
        }

        public Boolean ContainsForIDS(Kurse kursCheck) {
            Boolean retEqual = false;
            Boolean boolCheckFields = this.StrKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.DateBeginn.Equals(kursCheck.DateBeginn)
                        & this.DateEnde.Equals(kursCheck.DateEnde)
                        & this.StrOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant());

            if (this.StrKursNr.Length > 0)
                retEqual = boolCheckFields;
            else
                retEqual = boolCheckFields
                        & this.StrKursTitel.ToLowerInvariant()
                                .Replace("–", "-")
                                .Replace("/", "")
                                .Replace("ibm ", "")
                                .Replace(":", "").Trim()
                                .Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim());
            
            if(!retEqual && this.StrKursTitel.ToLowerInvariant()
                                    .Replace("–", "-")
                                    .Replace("/", "")
                                    .Replace("ibm ", "")
                                    .Replace(":", "").Trim()
                                    .Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim()) 
                                    && this.DateBeginn.Equals(kursCheck.DateBeginn) 
                                    && this.StrOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())) {

                if(!this.IPreis.Equals(kursCheck.IPreis) && this.StrKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant()))
                    kursCheck.StrReason = dictReason["Preis"];
                if(!this.BoolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && this.BoolGarantieTermin)
                    kursCheck.StrReason = dictReason["Garantie"];
                if(!this.BoolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && kursCheck.BoolGarantieTermin)
                    kursCheck.StrReason = dictReason["!Garantie"];
            } else if(!retEqual) {
                kursCheck.StrReason = dictReason["!Kurs"];
            }
            return retEqual;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    GC.SuppressFinalize(this);
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Kurse() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
