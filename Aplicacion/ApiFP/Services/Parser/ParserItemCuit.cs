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
                new Regex(@"^[2|3]\d{10}"),
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
                        bool esFormato16 = lineas[i].Contains("I.V.A.   : CF - CUIT");

                        string[] palabras = lineas[i].Split();
                        String cuit = encontrarSiguientePalabra(palabras, PALABRA_CLAVE_CUIT).Replace("-", "").Replace("/", "");

                        if (!esFormato16)
                            datosExtraidos.Cuit_Origen = cuit;
                        else
                            datosExtraidos.Cuit_Destino = cuit;

                        _primerCuitEncontrado = true;
                        continue;
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
                                        /*
                                    case 2:
                                        var palabras = lineas[i].Split();
                                        datosExtraidos.Cuit_Origen = palabras[palabras.Length - 1].Trim();
                                        break;
                                        */
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
                        //continue;

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
                                        /*
                                    case 2:
                                        var palabras = lineas[i].Split();
                                        datosExtraidos.Cuit_Origen = palabras[palabras.Length - 1].Trim();
                                        break;
                                        */
                                }
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