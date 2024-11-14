using NopCommerce.Models.CDEK;

namespace Nop.Web.Models;

public class OrderCuirer
{
    public string CdekNumber { get; set; }
    public string IntakeDate { get; set; }
    public string IntakeTimeFrom { get; set; }
    public string IntakeTimeTo { get; set; }

    public string Name { get; set; }
    public int? Weight { get; set; }
    public int? Length { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    public string Comment { get; set; }

    public Contact Sender { get; set; }

    public Location1 FromLocation { get; set; }

    public bool NeedCall { get; set; }
}

public class Contact
{
    public string Company { get; set; }
    public string Name { get; set; }
    public List<Phone> Phones { get; set; }
}

public class Location1
{
    public string Code { get; set; }
    public Guid FiasGuid { get; set; }
    public string PostalCode { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string CountryCode { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    public string City { get; set; }
    public string KladrCode { get; set; }
    public string Address { get; set; }
}
