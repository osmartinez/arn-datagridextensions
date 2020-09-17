namespace DataGridExtensions
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public enum FilterType
    {
        None = 0,
        Palabra = 1,
        Frase = 2,
        NumeroRango = 3,
        NumeroMenorque = 4,
        NumeroMayorque = 5,
        FechaEntera = 6,
        FechaDiaMes = 7,
        FechaDia = 8,
        EntreFechasUnMes = 9,
        EntreFechasMeses = 10,
    }

    /// <summary>
    /// A content filter using a simple "contains" string comparison to match the content and the value.
    /// </summary>
    public class SimpleContentFilter : IContentFilter
    {
        private readonly string _content;
        private readonly StringComparison _stringComparison;


        private FilterType GetFilterTypeDate(string filter, out int year, out int month, out int day,out int dayUntil, out int mesUntil)
        {
            Regex regexFechaEntera = new Regex(@"^(\d\d?)[/\-.](\d\d?)[/\-.](\d\d)$");
            Regex regexFechaDiaMes = new Regex(@"^(\d\d?)[/\-.](\d\d?)$");
            Regex regexFechaDia = new Regex(@"^(\d\d?)$");
            Regex regexEntreFechasUnMes = new Regex(@"^(\d\d?)[/\-.](\d\d?)(..|,,)(\d\d?)$");
            Regex regexEntreFechasMeses = new Regex(@"^(\d\d?)[/\-.](\d\d?)(..|,,)(\d\d?)[/\-.](\d\d?)$");

            year = -1;
            month = -1;
            day = -1;
            dayUntil = -1;
            mesUntil = -1;

            Match mRegexFechaEntera = regexFechaEntera.Match(filter);
            if (mRegexFechaEntera.Success)
            {
                day = Convert.ToInt32(mRegexFechaEntera.Groups[1].Value);
                month = Convert.ToInt32(mRegexFechaEntera.Groups[2].Value);
                year = Convert.ToInt32(mRegexFechaEntera.Groups[3].Value);

                return FilterType.FechaEntera;
            }

            Match mRegexFechaDiaMes = regexFechaDiaMes.Match(filter);
            if (mRegexFechaDiaMes.Success)
            {
                day = Convert.ToInt32(mRegexFechaDiaMes.Groups[1].Value);
                month = Convert.ToInt32(mRegexFechaDiaMes.Groups[2].Value);

                return FilterType.FechaDiaMes;
            }

            Match mRegexFechaDia = regexFechaDia.Match(filter);
            if (mRegexFechaDia.Success)
            {
                day = Convert.ToInt32(mRegexFechaDia.Groups[0].Value);

                return FilterType.FechaDia;
            }

            Match mRegexEntreFechasUnMes = regexEntreFechasUnMes.Match(filter);
            if (mRegexEntreFechasUnMes.Success)
            {
                day = Convert.ToInt32(mRegexEntreFechasUnMes.Groups[1].Value);
                month = Convert.ToInt32(mRegexEntreFechasUnMes.Groups[2].Value);
                dayUntil = Convert.ToInt32(mRegexEntreFechasUnMes.Groups[4].Value);
                return FilterType.EntreFechasUnMes;
            }

            Match mRegexEntreFechasMeses= regexEntreFechasMeses.Match(filter);
            if (mRegexEntreFechasMeses.Success)
            {
                day = Convert.ToInt32(mRegexEntreFechasMeses.Groups[1].Value);
                month = Convert.ToInt32(mRegexEntreFechasMeses.Groups[2].Value);
                dayUntil = Convert.ToInt32(mRegexEntreFechasMeses.Groups[4].Value);
                mesUntil = Convert.ToInt32(mRegexEntreFechasMeses.Groups[5].Value);

                return FilterType.EntreFechasMeses;
            }

            day = -1;
            month = -1;
            year = -1;
            return FilterType.None;
        }
        private FilterType GetFilterType(string filter)
        {
            filter = filter.Trim();
            Regex regexPalabra = new Regex(@"^[a-zA-Z0-9_]+$");
            Regex regexFrase = new Regex(@"([a-zA-Z0-9]+\s)+([a-zA-Z0-9]*)?");
            Regex regexRangoNumero = new Regex(@"(\d+[,.]?\d+)[.][.](\d+[,.]?\d+)");
            Regex regexNumeroMenorque = new Regex(@"[<](\d+[,.]?\d+)");
            Regex regexNumeroMayorque = new Regex(@"[>](\d+[,.]?\d+)");

            if (regexPalabra.Match(filter).Success)
            {
                return FilterType.Palabra;
            }
            else if (regexFrase.Match(filter).Success)
            {
                return FilterType.Frase;
            }
            else if (regexRangoNumero.Match(filter).Success)
            {
                return FilterType.NumeroRango;
            }
            else if (regexNumeroMenorque.Match(filter).Success)
            {
                return FilterType.NumeroMenorque;
            }
            else if (regexNumeroMayorque.Match(filter).Success)
            {
                return FilterType.NumeroMayorque;
            }
            else
            {
                return FilterType.None;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContentFilter"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="stringComparison">The string comparison.</param>
        public SimpleContentFilter(string content, StringComparison stringComparison)
        {
            _content = content;
            _stringComparison = stringComparison;
        }

        #region IFilter Members

        /// <summary>
        /// Determines whether the specified value matches the condition of this filter.
        /// </summary>
        /// <param name="value">The content.</param>
        /// <returns>
        ///   <c>true</c> if the specified value matches the condition; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(object? value)
        {
            if (value == null)
                return false;

            bool resultado = false;

            DateTime? fecha = value as DateTime?;
            if(fecha.HasValue)
            {
                int year = -1;
                int month = -1;
                int day = -1;
                int dayUntil = -1;
                int monthUntil = -1;
                FilterType tipoFiltro = GetFilterTypeDate(_content, out year, out month, out day, out dayUntil, out monthUntil);
                DateTime ahora = DateTime.Now;
                switch (tipoFiltro)
                {

                    case FilterType.FechaEntera:
                        {
                            resultado = fecha.Value.Day == day && fecha.Value.Month == month && fecha.Value.Year == year;
                            break;
                        }
                    case FilterType.FechaDiaMes:
                        {
                            resultado = fecha.Value.Day == day && fecha.Value.Month == month && fecha.Value.Year == ahora.Year;
                            break;
                        }
                    case FilterType.FechaDia:
                        {
                            resultado = fecha.Value.Day == day && fecha.Value.Month == ahora.Month && fecha.Value.Year == ahora.Year;
                            break;
                        }
                    case FilterType.EntreFechasUnMes:
                        {
                            DateTime fechaInicio = new DateTime(ahora.Year, month, day);
                            DateTime fechaFin = fechaInicio.Date;
                            if(day >= dayUntil)
                            {
                                fechaFin = fechaFin.AddDays(DateTime.DaysInMonth(ahora.Year,month) - (day - dayUntil) );
                            }
                            else
                            {
                                fechaFin = fechaFin.AddDays(dayUntil - day );
                            }
                            resultado = fechaInicio <= fecha.Value.Date && fecha.Value.Date <= fechaFin;
                            break;
                        }
                    case FilterType.EntreFechasMeses:
                        {
                            DateTime fechaInicio = new DateTime(ahora.Year, month, day);
                            DateTime fechaFin = new DateTime(month<=monthUntil? ahora.Year: ahora.Year+1, monthUntil, dayUntil);
                            resultado = fechaInicio <= fecha.Value.Date && fecha.Value.Date <= fechaFin;
                            break;
                        }
                    case FilterType.None:
                        break;

                }
            }
            else
            {
                FilterType tipoFiltro = GetFilterType(_content);
                switch (tipoFiltro)
                {
                    case FilterType.None:
                        resultado = value.ToString()?.Replace(",", ".").IndexOf(_content.Replace(",", "."), _stringComparison) >= 0;
                        break;
                    case FilterType.Palabra:
                        resultado = value.ToString()?.Replace(",", ".").IndexOf(_content, _stringComparison) >= 0;
                        break;
                    case FilterType.Frase:
                        {
                            // tipo string
                            string[] splittedTarget = value.ToString().Split(' ');
                            string[] splittedSource = _content.ToString().Split(' ');
                            bool[] results = new bool[splittedSource.Length];

                            for (int i = 0; i < splittedSource.Length; i++)
                            {
                                string source = splittedSource[i];
                                for (int j = 0; j < splittedTarget.Length; j++)
                                {
                                    string target = splittedTarget[j];
                                    if (target.IndexOf(source, _stringComparison) >= 0)
                                    {
                                        results[i] = true;
                                        break;
                                    }
                                    else
                                    {
                                        results[i] = false;
                                    }
                                }
                            }

                            bool match = true;
                            foreach (bool result in results)
                            {
                                match = match && result;
                            }
                            resultado = match;
                            break;
                        }
                    case FilterType.NumeroRango:
                        string[] tokens = _content.Split(new[] { ".." }, StringSplitOptions.None);
                        if (tokens.Length == 2)
                        {
                            decimal num1 = 0M;
                            decimal num2 = 0M;

                            bool parseados = false;
                            tokens[0] = tokens[0].Replace(',', '.');
                            tokens[1] = tokens[1].Replace(',', '.');


                            if (decimal.TryParse(tokens[0], NumberStyles.Any, CultureInfo.InvariantCulture, out num1) && decimal.TryParse(tokens[1], NumberStyles.Any, CultureInfo.InvariantCulture, out num2))
                            {
                                parseados = true;
                            }


                            if (parseados)
                            {
                                decimal valueDecimal = 0M;
                                if (decimal.TryParse(value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valueDecimal))
                                {
                                    resultado = num1 <= valueDecimal && valueDecimal <= num2;
                                }
                                else
                                {
                                    resultado = false;
                                }
                            }
                            else
                            {
                                resultado = false;
                            }
                        }
                        break;
                    case FilterType.NumeroMenorque:
                        {
                            string content = _content.Replace("<", "").Replace(',', '.');
                            decimal filter = 0M;
                            if (decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out filter))
                            {
                                decimal valueDecimal = 0M;
                                if (decimal.TryParse(value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valueDecimal))
                                {
                                    resultado = valueDecimal <= filter;
                                }
                                else
                                {
                                    resultado = false;
                                }
                            }

                            break;
                        }


                    case FilterType.NumeroMayorque:
                        {
                            string content = _content.Replace(">", "").Replace(',', '.');
                            decimal filter = 0M;
                            if (decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out filter))
                            {
                                decimal valueDecimal = 0M;
                                if (decimal.TryParse(value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valueDecimal))
                                {
                                    resultado = valueDecimal >= filter;
                                }
                                else
                                {
                                    resultado = false;
                                }
                            }

                            break;
                        }

                }
            }

            
            return resultado;
        }

        #endregion
    }

    /// <summary>
    /// Factory to create a <see cref="SimpleContentFilter"/>
    /// </summary>
    public class SimpleContentFilterFactory : IContentFilterFactory
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly IContentFilterFactory Default = new SimpleContentFilterFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContentFilterFactory"/> class.
        /// </summary>
        public SimpleContentFilterFactory()
            : this(StringComparison.CurrentCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContentFilterFactory"/> class.
        /// </summary>
        /// <param name="stringComparison">The string comparison to use.</param>
        public SimpleContentFilterFactory(StringComparison stringComparison)
        {
            StringComparison = stringComparison;
        }

        /// <summary>
        /// Gets or sets the string comparison.
        /// </summary>
        public StringComparison StringComparison
        {
            get;
            set;
        }

        #region IFilterFactory Members

        /// <summary>
        /// Creates the content filter for the specified content.
        /// </summary>
        /// <param name="content">The content to create the filter for.</param>
        /// <returns>
        /// The new filter.
        /// </returns>
        public IContentFilter Create(object? content)
        {
            return new SimpleContentFilter(content?.ToString() ?? string.Empty, StringComparison);
        }

        #endregion
    }
}
