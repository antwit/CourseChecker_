using NLog;
using System;
using System.Collections.Generic;

namespace CourseChecker.Course {

    /// <summary>
    /// Diese Klasse speichert alle nötige Informationen zum Kurs, sodass man später die Kurse vergleichen kann 
    /// </summary>
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

        /// <summary>
        /// Alle Gründe werden hier aufgelistet 
        /// </summary>
        private static readonly Dictionary<String, String> dictReason = new Dictionary<string, string>() {
            { "Preis", "Kurspreis wurde geändert;"},
            { "Garantie", "Kurs ist nun ein Garantietermin;" },
            { "!Garantie", "Kurs ist nicht mehr ein Garantietermin;" },
            { "!Kurs", "Kurs entfernt;" },
            { "Kurs", "Kurs hinzugefügt;" }
        };
        
        /// <summary>
        /// Konstruktor der Klasse ohne Angabe von Garantietermin 
        /// </summary>
        /// <param name="strKursNr">Kurs Nummer</param>
        /// <param name="strKursTitel">Kurs Titel</param>
        /// <param name="dateBeginn">Terminbeginn</param>
        /// <param name="dateEnde">Terminende</param>
        /// <param name="strOrt">Ort der Veranstaltung</param>
        /// <param name="iPreis">Preis ohne MwSt.</param>
        /// <param name="strAnbieter">Anbieter des Kurses</param>
        /// <param name="link">Link zum Kurs</param>
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

        /// <summary>
        /// Konstruktor der Klasse ohne Angabe von Garantietermin 
        /// </summary>
        /// <param name="strKursNr">Kurs Nummer</param>
        /// <param name="strKursTitel">Kurs Titel</param>
        /// <param name="dateBeginn">Terminbeginn</param>
        /// <param name="dateEnde">Terminende</param>
        /// <param name="boolGarantieTermin">Termin ist Garantietermin</param>
        /// <param name="strOrt">Ort der Veranstaltung</param>
        /// <param name="iPreis">Preis ohne MwSt.</param>
        /// <param name="strAnbieter">Anbieter des Kurses</param>
        /// <param name="link">Link zum Kurs</param>
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
        
        /// <summary>
        /// Prüft zwei Kurse auf Unterschiede
        /// </summary>
        /// <param name="kursCheck">Der Kurs mit dem verglichen wird</param>
        /// <returns>Falls die Kurse gleich sind gibt es ein "true", andernfalls ein "false" als boolischer Wert zurück</returns>
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

                //Gründe werden hinzugefügt
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

        /// <summary>
        /// Prüft zwei Kurse auf Unterschiede, hier speziell für IDS
        /// </summary>
        /// <param name="kursCheck">Der Kurs mit dem verglichen wird</param>
        /// <returns>Falls die Kurse gleich sind gibt es ein "true", andernfalls ein "false" als boolischer Wert zurück</returns>
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

                //Gründe werden hinzugefügt
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
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    GC.SuppressFinalize(this);
                }
                disposedValue = true;
            }
        }

        ~Kurse() {
            Dispose(false);
        }
        
        void IDisposable.Dispose() {
            Dispose(true);
        }
        #endregion
    }
}
