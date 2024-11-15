namespace NopCommerce.Models.CDEK;

using System;
using System.Collections.Generic;

public class Order
{
    public string number { get; set; }
    public string comment { get; set; }
    public DeliveryRecipientCost deliveryRecipientCost { get; set; }
    public List<DeliveryRecipientCostAdv> deliveryRecipientCostAdv { get; set; }
    public Location fromLocation { get; set; }
    public Location toLocation { get; set; }
    public List<Package> packages { get; set; }
    public Recipient recipient { get; set; }
    public Sender sender { get; set; }
    public List<Service> services { get; set; }
    public int tariffCode { get; set; }
}

public class DeliveryRecipientCost
{
    public decimal value { get; set; }
}

public class DeliveryRecipientCostAdv
{
    public decimal sum { get; set; }
    public decimal threshold { get; set; }
}

public class Location
{
    public string code { get; set; }
    public string fiasGuid { get; set; }
    public string postalCode { get; set; }
    public string longitude { get; set; }
    public string latitude { get; set; }
    public string countryCode { get; set; }
    public string region { get; set; }
    public string subRegion { get; set; }
    public string city { get; set; }
    public string kladrCode { get; set; }
    public string address { get; set; }
}

public class Package
{
    public string number { get; set; }
    public string comment { get; set; }
    public int height { get; set; }
    public List<Items> items { get; set; }
    public int length { get; set; }
    public int weight { get; set; }
    public int width { get; set; }
}

public class Items
{
    public string wareKey { get; set; }
    public Payment payment { get; set; }
    public string name { get; set; }
    public decimal cost { get; set; }
    public int amount { get; set; }
    public int weight { get; set; }
    public string url { get; set; }
}

public class Payment
{
    public decimal value { get; set; }
}

public class Recipient
{
    public string name { get; set; }
    public List<Phone> phones { get; set; }
}

public class Phone
{
    public string number { get; set; }
}

public class Sender
{
    public string name { get; set; }
}

public class Service
{
    public string code { get; set; }
}
