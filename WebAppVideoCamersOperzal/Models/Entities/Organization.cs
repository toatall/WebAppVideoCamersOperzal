using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebAppVideoCamersOperzal.Models.Entities
{
    
    /// <summary>
    /// Организация
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Код организации
        /// </summary>
        [Key, Required(ErrorMessage = "Поле является обазательным для заполнения")]
        public string code { get; set; }

        /// <summary>
        /// Наименование 
        /// </summary>
        [Required(ErrorMessage = "Поле является обазательным для заполнения")]
        public string name { get; set; }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        public long? dateCreate { get; set; }


        public string? dateCreateStr
        {
            get
            {
                if (dateCreate != null)
                {

                    DateTime d = new DateTime(1970, 1, 1).AddSeconds(double.Parse(dateCreate.ToString()));
                    return d.ToString("yyyy-MM-dd");
                }
                return null;
            }
        }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// Дата реорганизации
        /// </summary>
        public long? dateEnd { get; set; }

        public string? dateEndStr 
        { 
            get
            {
                if (dateEnd != null)
                {

                    DateTime d = new DateTime(1970, 1, 1).AddSeconds(double.Parse(dateEnd.ToString()));
                    return d.ToString("yyyy-MM-dd");
                }
                return null;
            } 
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    DateTime d;
                    if (DateTime.TryParse(value, out d))
                    {
                        long dateEnd = new DateTimeOffset(d, TimeSpan.Zero).ToUnixTimeSeconds();
                        this.dateEnd = dateEnd;
                    }
                    else
                    {
                        dateEnd = null;
                    }
                }
                else
                {
                    dateEnd = null;
                }
            }
        }

        public int countCamersEnabled
        {
            get
            {
                if (camers != null)
                {
                    return camers.Where(e => e.disabled == false).Count();
                }
                return 0;                
            }
        }

        public int countCamersDisabled
        {
            get
            {
                if (camers != null)
                {
                    return camers.Where(e => e.disabled == true).Count();
                }
                return 0;
            }
        }

        public string fullName
        {
            get
            {
                return $"{name} ({code})";
            }
        }


        /// <summary>
        /// Камеры
        /// </summary>        
        public IEnumerable<VideoCamera> camers { get; set; }

    }

}
