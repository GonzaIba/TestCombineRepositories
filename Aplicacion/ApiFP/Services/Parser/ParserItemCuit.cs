using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ApiFP.Services.Parser
{
    public class ParserItemCuit : ParserItem
    {
        private string PALABRA_CLAVE_CUIT;
        private bool _primerCuitEncontrado;
        private List<Regex> _rxCuit;
        private Match _matchCuit;

        public ParserItemCuit()
        {
            PALABRA_CLAVE_CUIT = ConfigurationManager.AppSettings["PALABRA_CLAVE_CUIT"];
            _primerCuitEncontrado = false;

            _rxCuit = new List<Regex>()
            {
                new Regex(@"^\d{2}-\d{8}-\d{1}"),
                new Regex(@"^[1|2|3]\d{10}"),
                new Regex(@"\d{2}-\d{8}-\d{1}"),
                new Regex(@"\d{2}-\d{8}/\d{1}"),
                new Regex(@":[2|3]\d{10}")
            };            
        }

        override public void Parse(Business.DatosFactura datosExtraidos, String[] lineas)
        {
            for (int i = 0; i < lineas.Length; i++)
            {
                #region "CUIT_ORIGEN"                
                if (String.IsNullOrEmpty(datosExtraidos.Cuit_Origen))
                {
                    if (lineas[i].Contains(PALABRA_CLAVE_CUIT) && !_primerCuitEncontrado)
                    {
                        bool esCUITDestino = (lineas[i].Contains("I.V.A.   : CF - CUIT")) ||
                            (lineas[i - 2].ToLower().Contains("situación iva:")) ||
                            (lineas[i + 1].ToLower().Contains("evolución"));

                        string[] palabras = lineas[i].Split();
                        String cuit = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT).Replace("-", "").Replace("/", "");

                        if (_rxCuit[1].IsMatch(cuit))
                        {
                            if (!esCUITDestino)
                                datosExtraidos.Cuit_Origen = cuit;
                            else
                                datosExtraidos.Cuit_Destino = cuit;

                            _primerCuitEncontrado = true;
                            continue;
                        }
                    }

                    if (String.IsNullOrEmpty(datosExtraidos.Cuit_Origen))
                    {
                        foreach (var exp in _rxCuit.Select((value, idx) => new { value, idx }))
                        {
                            _matchCuit = exp.value.Match(lineas[i].Trim());
                            if (_matchCuit.Success)
                            {
                                switch (exp.idx)
                                {
                                    case 0:
                                    case 2:
                                    case 3:
                                        datosExtraidos.Cuit_Origen = _matchCuit.Value.Replace("-", "").Replace("/", "");
                                        break;
                                    case 1:
                                        datosExtraidos.Cuit_Origen = _matchCuit.Value;
                                        break;
                                    case 4:
                                        datosExtraidos.Cuit_Origen = _matchCuit.Value.Replace(":", "");
                                        break;
                                }
                                _primerCuitEncontrado = !String.IsNullOrEmpty(datosExtraidos.Cuit_Origen);
                            }
                        }
                        if (_primerCuitEncontrado)
                            continue;
                    }
                }
                #endregion

                #region "CUIT_DESTINO"
                if (_primerCuitEncontrado)
                {
                    if (lineas[i].Contains(PALABRA_CLAVE_CUIT))
                    {
                        if (String.IsNullOrEmpty(datosExtraidos.Cuit_Destino))
                        {
                            string[] palabras = lineas[i].Split();
                            datosExtraidos.Cuit_Destino = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT);

                            if (lineas[i].Contains("Nombre: Documento / CUIT: "))
                            {
                                var line = lineas[i - 1].Split();
                                datosExtraidos.Cuit_Destino = line[line.Length - 1];
                            }
                        }
                        datosExtraidos.Cuit_Destino = datosExtraidos.Cuit_Destino.Replace("-", "").Replace("/", "");

                        if (String.IsNullOrEmpty(datosExtraidos.Cuit_Destino) == false)
                            return;
                    }

                    if (String.IsNullOrEmpty(datosExtraidos.Cuit_Destino))
                    {
                        foreach (var exp in _rxCuit.Select((value, idx) => new { value, idx }))
                        {
                            _matchCuit = exp.value.Match(lineas[i].Trim());
                            if (_matchCuit.Success)
                            {
                                switch (exp.idx)
                                {
                                    case 0:
                                    case 2:
                                    case 3:
                                        datosExtraidos.Cuit_Destino = _matchCuit.Value.Replace("-", "").Replace("/", "");
                                        break;
                                    case 1:
                                        datosExtraidos.Cuit_Destino = _matchCuit.Value;
                                        break;
                                    case 4:
                                        datosExtraidos.Cuit_Destino = _matchCuit.Value.Replace(":", "");
                                        break;
                                }
                                if (datosExtraidos.Cuit_Origen == datosExtraidos.Cuit_Destino)
                                    datosExtraidos.Cuit_Destino = String.Empty;
                                else
                                    return;
                            }
                        }
                    }
                }
                #endregion

            }
        }
    }
}