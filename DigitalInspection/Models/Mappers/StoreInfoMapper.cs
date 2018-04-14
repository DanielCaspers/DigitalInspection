using DigitalInspection.Models.Store;
using System.Collections.Generic;
using DigitalInspection.Models.DTOs;

namespace DigitalInspection.Models.Mappers
{
	public static class StoreInfoMapper
	{
		public static StoreInfo mapToStoreInfo(StoreInfoDTO dto)
		{
			StoreInfo storeInfo = new StoreInfo
			{
				Name = dto.coname,
				NameShort = dto.conameShort,
				ManagerFirstName = dto.comgrfirstname,
				ManagerLastName = dto.comgrlastname,
				CodeName = dto.cogmem,

				PhoneNumberToCall = new WebPhoneNumber(
					dto.conameShort,
					dto.cophone,
					dto.codial),

				PhoneNumberToSMS = new WebPhoneNumber(
					dto.conameShort,
					dto.cosms,
					dto.cosms),

				StoreAddress = new StoreAddress(
					dto.coaddr,
					dto.cocity,
					dto.costateL,
					dto.costate,
					dto.cozip,
					dto.latitude,
					dto.longitude,
					dto.loc_near),

				CommunitiesServed = new List<string>
				{
					dto.coserv1,
					dto.coserv2,
					dto.coserv3,
					dto.coserv4
				},

				StoreFeatures = new StoreFeatures(
					dto.cohr_mf,
					dto.cohr_sat,
					dto.num_bays,
					dto.num_techs,
					dto.tow_coname,
					dto.tow_phone,
					dto.towdial),

				StoreIntegrations = new StoreIntegrations(
					dto.mapquestlink,
					dto.FaceBookURL,
					dto.FaceBookBadge,
					dto.GooglePlusID,
					dto.GoogleSiteURL,
					dto.GoogleReviewURL,
					dto.GooglePlacesRef,
					dto.YahooSiteURL,
					dto.YelpSiteURL,
					dto.InsiderPagesURL,
					dto.DexSiteURL),

				StoreWebAssets = new StoreWebAssets(
					dto.webaddr,
					dto.sitemapURL,
					dto.logoSmall)
			};

			return storeInfo;
		}
	}
}
