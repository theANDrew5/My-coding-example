using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models.DDelivery
{
	public class DDeliveryCalculatorResult
	{
		public string DeliveryCompany { get; set; }
		public string DeliveryCompanyName { get; set; }
		public string DeliveryCompanyDriverVersion { get; set; }

		public string PickupCompanyDriverVersion { get; set; }
		public decimal PickupPrice { get; set; }
		public decimal DeliveryPrice { get; set; }
		public decimal DeliveryPriceFee { get; set; }
		public decimal DeclaredPriceFee { get; set; }

		public int DeliveryTimeMin { get; set; }
		public int DeliveryTimeAvg { get; set; }
		public int DeliveryTimeMax { get; set; }

		//public double ReturnPrice { get; set; }
		//public double ReturnClientPrice { get; set; }

		//public double ReturnPartialPrice { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal PaymentPriceFee { get; set; }

		public string DeliveryDate { get; set; }

		public string ConfirmDate { get; set; }

		public string PickupDate { get; set; }
		public bool PaymentAvailability { get; set; }

		public string Name
		{
			get
			{
				int id;
				if (int.TryParse(DeliveryCompany, out id) && Info.ContainsKey(id))
				{
					return Info[id].Item1;
				}
				return null;
			}
		}
		public string Ico
		{
			get
			{
				int id;
				if (int.TryParse(DeliveryCompany, out id) && Info.ContainsKey(id))
				{
					return Info[id].Item2;
				}
				return null;
			}
		}

		public decimal TotalPriceCorrected { get; set; }
		public int DeliveryTimeMinCorrected { get; set; }
		public int DeliveryTimeAvgCorrected { get; set; }
		public int DeliveryTimeMaxCorrected { get; set; }

		private static readonly Dictionary<int, Tuple<string, string>> Info = new Dictionary<int, Tuple<string, string>>
			{
					{1, new Tuple<string, string>("PickPoint", "pickpoint")},
					{3, new Tuple<string, string>("Logibox", "logibox")},
					{4, new Tuple<string, string>("Boxberry", "boxberry")},
					{6, new Tuple<string, string>("СДЭК забор", "cdek")},
                    {7, new Tuple<string, string>("QIWI Post", "qiwi")},
                    {11, new Tuple<string, string>("Hermes", "hermes")},
                    {13, new Tuple<string, string>("КТС", "pack")},
                    {14, new Tuple<string, string>("Maxima Express", "pack")},
                    {16, new Tuple<string, string>("IMLogistics Пушкинская", "imlogistics")},
                    {17, new Tuple<string, string>("IMLogistics", "imlogistics")},
                    {18, new Tuple<string, string>("Сам Заберу", "pack")},
                    {20, new Tuple<string, string>("DPD Parcel", "dpd")},
                    {21, new Tuple<string, string>("Boxberry Express", "boxberry")},
                    {22, new Tuple<string, string>("IMLogistics Экспресс", "imlogistics")},
                    {23, new Tuple<string, string>("DPD Consumer", "dpd")},
                    {24, new Tuple<string, string>("Сити Курьер", "pack")},
                    {25, new Tuple<string, string>("СДЭК Посылка Самовывоз", "cdek")},
                    {26, new Tuple<string, string>("СДЭК Посылка до двери", "cdek")},
                    {27, new Tuple<string, string>("DPD ECONOMY", "dpd")},
                    {28, new Tuple<string, string>("DPD Express", "dpd")},
                    {29, new Tuple<string, string>("DPD Classic", "dpd")},
                    {30, new Tuple<string, string>("EMS", "ems")},
                    {31, new Tuple<string, string>("Grastin", "grastin")},
                    {33, new Tuple<string, string>("Aplix", "aplix")},
                    {34, new Tuple<string, string>("Lenod", "pack")},
                    {35, new Tuple<string, string>("Aplix DPD Consum er", "aplix_dpd_black")},
                    {36, new Tuple<string, string>("Aplix DPD parcel", "aplix_dpd_black")},
                    {37, new Tuple<string, string>("Aplix IML самовывоз", "aplix_imlogistics")},
                    {38, new Tuple<string, string>("Aplix PickPoint", "aplix_pickpoint")},
                    {39, new Tuple<string, string>("Aplix Qiwi", "aplix_qiwi")},
                    {40, new Tuple<string, string>("Aplix СДЭК", "aplix_cdek")},
                    {41, new Tuple<string, string>("Кит", "kit")},
                    {42, new Tuple<string, string>("Imlogistics", "imlogistics")},
                    {43, new Tuple<string, string>("Imlogistics", "imlogistics")},
                    {44, new Tuple<string, string>("Почта России", "russianpost")},
                    {45, new Tuple<string, string>("Aplix курьерская доставка", "aplix")},
                    {46, new Tuple<string, string>("Lenod", "pack")},
                    {47, new Tuple<string, string>("Телепост", "pack")},
                    {48, new Tuple<string, string>("Aplix IML курьерская доставка", "aplix_imlogistics")},
                    {49, new Tuple<string, string>("IML Забор", "imlogistics")},
                    {50, new Tuple<string, string>("Почта России 1-й класс", "mail")},
                    {51, new Tuple<string, string>("EMS Почта России", "ems")},
                    {52, new Tuple<string, string>("ЕКБ-доставка забор", "pack")},
                    {53, new Tuple<string, string>("Грейт Экспресс", "pack")},
                    {54, new Tuple<string, string>("Почта России 1-й класс.", "mail")},
                    {55, new Tuple<string, string>("Почта России.", "mail")},
                    {58, new Tuple<string, string>("FSD - курьерская доставка по Москве", "pack")},
                    {61, new Tuple<string, string>("EMS Почта России", "ems")},
					{88, new Tuple<string, string>("Почта России B2CPL", "russianpost") },
					{89, new Tuple<string, string>("Почта России 1-й класс B2CPL", "russianpost") },
					{90, new Tuple<string, string>("Почта России Shop-Logistics", "russianpost") },
					{91, new Tuple<string, string>("Почта России 1-й класс Shop-Logistics", "russianpost") },
					{92, new Tuple<string, string>("Достависта", "pack") }

			};
	}


}