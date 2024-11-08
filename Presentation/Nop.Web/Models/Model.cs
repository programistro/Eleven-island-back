namespace NopCommerce.Models;

using System;
using System.Collections.Generic;

public class Payment
{
    public string TerminalKey { get; set; }
    public int Amount { get; set; }
    public string OrderId { get; set; }
    public string? Description { get; set; }
    public string Token { get; set; }
    public Data DATA { get; set; }
    public Receipt Receipt { get; set; }
}

public class Data
{
    public string? Phone { get; set; }
    public string Email { get; set; }
}

public class Receipt
{
    public string Email { get; set; }
    
    public string Phone { get; set; }
    /// <summary>
    /// ндс
    /// </summary>
    public string Taxation { get; set; }
    public List<Item> Items { get; set; }
}

public class Item
{
    public string Name { get; set; }
    /// <summary>
    /// цена на сайте
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// сколько
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// в копейках
    /// </summary>
    public int Amount { get; set; }
    /// <summary>
    /// ставка налога
    /// </summary>
    public string Tax { get; set; }
    /// <summary>
    /// штрихкод
    /// </summary>
    public string? Ean13 { get; set; }
}
