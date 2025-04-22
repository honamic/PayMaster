using System.ComponentModel.DataAnnotations;

namespace Honamic.PayMaster.PaymentProvider.Sadad.Models;

public class PaymentRequest
{
    public required string TerminalId { get; set; }

    public required string MerchantId { get; set; }

    public decimal Amount { get; set; }

    public required string OrderId { get; set; }


    /// <summary>
    /// تاريخ و زمان ارسال تراکنش
    /// </summary>
    public DateTime LocalDateTime { get; set; }

    public required string ReturnUrl { get; set; }

    /// <summary>
    /// اطلاعات تراکنش به صورت رمزنگاری شده توسط کلید پذيرنده
    /// </summary>
    public required string SignData { get; set; }

    public string? AdditionalData { get; set; }

    /// <summary>
    /// شماره تلفن همراه دارنده کارت
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// نام اپلیکیشن درخواست کننده
    /// اختیاری - برای گزارشات لازم  است که اين فیلد مقدار دهی شود
    /// </summary>
    public string? ApplicationName { get; set; }


    /// <summary>
    /// نوع احراز هويت
    /// مقدار 0 هیچکدام 
    /// مقدار 1 احراز هويت با کد ملی
    /// مقدار 2 احراز هويت با موبايل
    /// مقدار 3 احراز هويت با کد ملی اينکريپت شده
    /// </summary>
    public short? PanAuthenticationType { get; set; }

    /// <summary>
    /// اختیاری در صورتیکه فیلد  PanAuthenticationType با  مقدار 1 ارسال شود 
    /// الزم است که کدملی دارنده کارت در اين فیلد ارسال شود
    /// </summary>
    public string? NationalCode { get; set; }

    /// <summary>
    /// در صورتیکه فیلد PanAuthenticationType با مقدار 3 ارسال شود
    /// الزم است که کد ملی دارنده کارت بصورت رمزنگاری شده در اين فیلد ارسال شود.
    /// </summary>
    public string? NationalCodeEnc { get; set; }

    /// <summary>
    /// شماره موبايل دارنده کارت
    /// اختیاری، در صورتیکه فیلد PanAuthenticationType با مقدار 2 ارسال شود 
    /// الزم است که موبايل دارنده کارت در اين فیلد ارسال شود
    /// </summary>
    public string? CardHolderIdentity { get; set; }

    ///// <summary>
    ///// لیستی از شماره کارت ها برای نمايش
    ///// </summary>
    //public object[]? SourcePanList { get; set; }

    [Display(Name = @"پرداخت تسهیم")]
    public bool EnableMultiplexing { get; set; }

    public PaymentRequestMultiplexingData? MultiplexingData { get; set; }
}


