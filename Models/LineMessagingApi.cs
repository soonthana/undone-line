using System;
using System.Collections.Generic;

namespace Undone.Line.BusinessModel
{
  public class LineMessagingApi
  {
    #region Line Webhook Request Body (Receiving from LINE)
    public class Webhook
    {
      public List<Event> Events { get; set; }
    }

    public class Event
    {
      /// <summary>
      /// "message" or "follow" or "unfollow" or "join" or "leave" or "postback" or "beacon"
      /// </summary>
      public string Type { get; set; }
      public long Timestamp { get; set; }
      public Sources Source { get; set; }
      public string ReplyToken { get; set; }
      public EventMessages Message { get; set; }
      public EventPostback Postback { get; set; }
      public EventBeacon Beacon { get; set; }
    }

    public class Sources
    {
      /// <summary>
      /// "user" or "group" or "room"
      /// </summary>
      public string Type { get; set; }
      public string UserId { get; set; }
      public string GroupId { get; set; }
      public string RoomId { get; set; }
    }

    public class EventMessages
    {
      public string Id { get; set; }
      /// <summary>
      /// "text" or "image" or "video" or "audio" or "file" or "location" or "sticker"
      /// </summary>
      public string Type { get; set; }
      public string Text { get; set; }
      public string FileName { get; set; }
      public string FileSize { get; set; }
      public string Title { get; set; }
      public string Address { get; set; }
      public decimal Latitude { get; set; }
      public decimal Longitude { get; set; }
      public string PackageId { get; set; }
      public string StickerId { get; set; }
    }

    public class EventFollow
    {

    }

    public class EventUnfollow
    {

    }

    public class EventJoin
    {

    }

    public class EventLeave
    {

    }

    public class EventPostback
    {
      public string Data { get; set; }
      public PostbackParams Params { get; set; }
    }

    public class PostbackParams
    {
      public string Date { get; set; }
      public string Time { get; set; }
      public string Datetime { get; set; }
    }

    public class EventBeacon
    {
      public string Hwid { get; set; }
      /// <summary>
      /// "enter" or "leave" or "banner"
      /// </summary>
      public string Type { get; set; }
      public string Dm { get; set; }
    }
    #endregion




    //#region NOTUSE Line Webhook Request Body (Receiving from LINE)
    ///// <summary>
    ///// Webhook Receiving from LINE
    ///// </summary>
    //public class Webhook9
    //{
    //    public Webhook9()
    //    {
    //        this.Events = new List<IEvent>() { new MessageEvent(), new FollowEvent(), new UnfollowEvent(), new JoinEvent(), new LeaveEvent(), new PostbackEvent(), new BeaconEvent() };
    //    }
    //    public List<IEvent> Events { get; set; }
    //}

    //public interface IEvent
    //{
    //    string Type { get; set; }
    //    int Timestamp { get; set; }
    //    ISource Source { get; set; }
    //}

    //#region Interfaces and Classes for Source
    //public interface ISource
    //{
    //    string Type { get; set; }
    //    string UserId { get; set; }
    //}

    //public class SourceUser : ISource
    //{
    //    public string Type { get; set; } = "user";
    //    public string UserId { get; set; }
    //}

    //public class SourceGroup : ISource
    //{
    //    public string Type { get; set; } = "group";
    //    public string GroupId { get; set; }
    //    public string UserId { get; set; }
    //}

    //public class SourceRoom : ISource
    //{
    //    public string Type { get; set; } = "room";
    //    public string RoomId { get; set; }
    //    public string UserId { get; set; }
    //}
    //#endregion

    //public class MessageEvent : IEvent
    //{
    //    public string Type { get; set; } = "message";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //    public string ReplyToken { get; set; }
    //    public IReceiveMessage Message { get; set; }
    //}

    //#region Interfaces and Classes for MessageEvent
    //public interface IReceiveMessage
    //{
    //    string Id { get; set; }
    //    string Type { get; set; }
    //}

    ///// <summary>
    ///// For Text to ReceiveMessage
    ///// </summary>
    //public class TextReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "text";
    //    public string Text { get; set; }
    //}

    ///// <summary>
    ///// For Image to ReceiveMessage
    ///// </summary>
    //public class ImageReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "image";
    //}

    ///// <summary>
    ///// For Video to ReceiveMessage
    ///// </summary>
    //public class VideoReceiveMessageTeyp : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "video";
    //}

    ///// <summary>
    ///// For Audio to ReceiveMessage
    ///// </summary>
    //public class AudioReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "audio";
    //}

    ///// <summary>
    ///// For File to ReceiveMessage
    ///// </summary>
    //public class FileReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "file";
    //    public string FileName { get; set; }
    //    public string FileSize { get; set; }
    //}

    ///// <summary>
    ///// For Location to ReceiveMessage
    ///// </summary>
    //public class LocationReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "location";
    //    public string Title { get; set; }
    //    public string Address { get; set; }
    //    public decimal Latitude { get; set; }
    //    public decimal Longitude { get; set; }
    //}

    ///// <summary>
    ///// For Sticker to ReceiveMessage
    ///// </summary>
    //public class StickerReceiveMessageType : IReceiveMessage
    //{
    //    public string Id { get; set; }
    //    public string Type { get; set; } = "sticker";
    //    public string PackageId { get; set; }
    //    public string StickerId { get; set; }
    //}
    //#endregion

    //public class FollowEvent : IEvent
    //{
    //    public string Type { get; set; } = "follow";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //    public string ReplyToken { get; set; }
    //}

    //public class UnfollowEvent : IEvent
    //{
    //    public string Type { get; set; } = "unfollow";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //}

    //public class JoinEvent : IEvent
    //{
    //    public string Type { get; set; } = "join";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //    public string ReplyToken { get; set; }
    //}

    //public class LeaveEvent : IEvent
    //{
    //    public string Type { get; set; } = "leave";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //}

    //public class PostbackEvent : IEvent
    //{
    //    public string Type { get; set; } = "postback";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //    public string ReplyToken { get; set; }
    //    public string PostbackData { get; set; }
    //    public string PostbackParams { get; set; }
    //}

    //public class BeaconEvent : IEvent
    //{
    //    public string Type { get; set; } = "beacon";
    //    public int Timestamp { get; set; }
    //    public ISource Source { get; set; }
    //    public string ReplyToken { get; set; }
    //    public string BeaconHwid { get; set; }
    //    public BeaconType BeaconType { get; set; }
    //    public string BeaconDm { get; set; }
    //}

    //#region Enums for BeaconEvent
    //public enum BeaconType
    //{
    //    Enter,
    //    Leave,
    //    Banner
    //}
    //#endregion
    //#endregion



    #region Line Reply Message Request Body (Sending to LINE)
    /// <summary>
    /// ReplyMessage Sending to LINE
    /// </summary>
    public class ReplyMessage
    {
      public string replyToken { get; set; }
      public List<SendMessage> messages { get; set; }
    }
    #endregion

    #region Line Push Message Request Body (Sending to LINE)
    /// <summary>
    /// PushMessage Sending to LINE
    /// </summary>
    public class PushMessage
    {
      public string to { get; set; }
      public List<SendMessage> messages { get; set; }
    }
    #endregion

    #region Line Multicast Message Request Body (Sending to LINE)
    /// <summary>
    /// MulticastMessage Sending to LINE
    /// </summary>
    public class MulticastMessage
    {
      public List<string> to { get; set; }
      public List<SendMessage> messages { get; set; }
    }
    #endregion


    #region SendMessage Objects
    public class SendMessage
    {
      /// <summary>
      /// "text" or "image" or "video" or "audio" or "location" or "sticker" or "imagemap" or "template"
      /// </summary>
      public string type { get; set; }
      public string text { get; set; }
      public string packageId { get; set; }
      public string stickerId { get; set; }
      public string originalContentUrl { get; set; }
      public string previewImageUrl { get; set; }
      public int duration { get; set; }
      public string title { get; set; }
      public string address { get; set; }
      public decimal latitude { get; set; }
      public decimal longitude { get; set; }
      public string baseUrl { get; set; }
      public string altText { get; set; }
      public ImagemapBaseSize baseSize { get; set; }
      public ImagemapAction actions { get; set; }
      public Template template { get; set; }
    }

    public class ImagemapBaseSize
    {
      public string width { get; set; }
      public string height { get; set; }
    }

    public class ImagemapAction
    {
      /// <summary>
      /// "uri" or "message"
      /// </summary>
      public string type { get; set; }
      public string linkUri { get; set; }
      public ImagemapArea area { get; set; }
      public string text { get; set; }
    }

    public class ImagemapArea
    {
      public int x { get; set; }
      public int y { get; set; }
      public int width { get; set; }
      public int height { get; set; }
    }

    public class Template
    {
      /// <summary>
      /// "buttons" or "confirm" or "carousel" or "image_carousel"
      /// </summary>
      public string type { get; set; }
      public string thumbnailImageUrl { get; set; }
      public string title { get; set; }
      public string text { get; set; }
      public List<TemplateAction> actions { get; set; }
      public List<Columns> columns { get; set; }
    }

    public class TemplateAction
    {
      /// <summary>
      /// "postback" or "message" or "uri" or "datetimepicker"
      /// </summary>
      public string type { get; set; }
      public string label { get; set; }
      public string data { get; set; }
      public string text { get; set; }
      /// <summary>
      /// "http" or "https" or "tel"
      /// </summary>
      public string uri { get; set; }
      /// <summary>
      /// "yyyy-MM-dd" or "hh:mm" or "yyyy-MM-ddThh:mm"
      /// </summary>
      public string mode { get; set; }
      public string initial { get; set; }
      public string max { get; set; }
      public string min { get; set; }
    }

    public class Columns
    {
      public string imageUrl { get; set; }
      public TemplateAction action { get; set; }
      public string thumbnailImageUrl { get; set; }
      public string title { get; set; }
      public string text { get; set; }
      public List<TemplateAction> actions { get; set; }
    }
    #endregion








    //#region NOTUSE SendMessage Objects
    //public interface ISendMessage
    //{
    //}

    ///// <summary>
    ///// For Text to SendMessage
    ///// </summary>
    //public class TextSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "text";
    //    public string Text { get; set; }
    //}

    ///// <summary>
    ///// For Image to SendMessage
    ///// </summary>
    //public class ImageSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "image";
    //    public string OriginalContentUrl { get; set; }
    //    public string PreviewImageUrl { get; set; }
    //}

    ///// <summary>
    ///// For Video to SendMessage
    ///// </summary>
    //public class VideoSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "video";
    //    public string OriginalContentUrl { get; set; }
    //    public string PreviewImageUrl { get; set; }
    //}

    ///// <summary>
    ///// For Audio to SendMessage
    ///// </summary>
    //public class AudioSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "audio";
    //    public string OriginalContentUrl { get; set; }
    //    public int Duration { get; set; }
    //}

    ///// <summary>
    ///// For Location to SendMessage
    ///// </summary>
    //public class LocationSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "locaton";
    //    public string Title { get; set; }
    //    public string Address { get; set; }
    //    public decimal Latitude { get; set; }
    //    public decimal Longitude { get; set; }
    //}

    ///// <summary>
    ///// For Sticker to SendMessage
    ///// </summary>
    //public class StickerSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "sticker";
    //    public string PackageId { get; set; }
    //    public string StickerId { get; set; }
    //}

    ///// <summary>
    ///// For Imagemap to SendMessage
    ///// </summary>
    //public class ImagemapSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "imagemap";
    //    public string BaseUrl { get; set; }
    //    public string AltText { get; set; }
    //    public int BaseSizeWidth { get; set; }
    //    public int BaseSizeHeight { get; set; }
    //    public List<IImagemapAction> Actions { get; set; }
    //}

    //#region Interfaces and Classes for ImagemapSendMessageType
    //public interface IImagemapAction
    //{
    //    string Type { get; set; }
    //    ImagemapArea Area { get; set; }
    //}

    //public class ImagemapActionUri : IImagemapAction
    //{
    //    public string Type { get; set; } = "uri";
    //    public string LinkUri { get; set; }
    //    public ImagemapArea Area { get; set; }
    //}

    //public class ImagemapActionMessage : IImagemapAction
    //{
    //    public string Type { get; set; } = "message";
    //    public string Text { get; set; }
    //    public ImagemapArea Area { get; set; }
    //}

    //public class ImagemapArea
    //{
    //    public int X { get; set; }
    //    public int Y { get; set; }
    //    public int Width { get; set; }
    //    public int Height { get; set; }
    //}
    //#endregion

    ///// <summary>
    ///// For Template to SendMessage
    ///// </summary>
    //public class TemplateSendMessageType : ISendMessage
    //{
    //    public string Type { get; set; } = "template";
    //    public string AltText { get; set; }
    //    public ITemplateCategory Template { get; set; }
    //}

    //#region Interfaces and Classes for TemplateSendMessageType
    //public interface ITemplateCategory
    //{
    //    string Type { get; set; }
    //}

    //public class Buttons : ITemplateCategory
    //{
    //    public string Type { get; set; } = "buttons";
    //    public string ThumbnailImageUrl { get; set; }
    //    public string Title { get; set; }
    //    public string Text { get; set; }
    //    public List<ITemplateAction> Actions { get; set; }
    //}

    //public class Confirm : ITemplateCategory
    //{
    //    public string Type { get; set; } = "confirm";
    //    public string Text { get; set; }
    //    public List<ITemplateAction> Actions { get; set; }
    //}

    //public class Carousel : ITemplateCategory
    //{
    //    public string Type { get; set; } = "carousel";
    //    public List<CarouseColumnObject> Columns { get; set; }
    //}

    //public class ImageCarousel : ITemplateCategory
    //{
    //    public string Type { get; set; } = "image_carousel";
    //    public List<ImageCarouselColumnObject> Columns { get; set; }
    //}

    //public class CarouseColumnObject
    //{
    //    public string ThumbnailImageUrl { get; set; }
    //    public string Title { get; set; }
    //    public string Text { get; set; }
    //    public List<ITemplateAction> Actions { get; set; }
    //}

    //public class ImageCarouselColumnObject
    //{
    //    public string ImageUrl { get; set; }
    //    public ITemplateAction Action { get; set; }
    //}

    //public interface ITemplateAction
    //{
    //    string Type { get; set; }
    //    string Label { get; set; }
    //}

    //public class TemplateActionPostback : ITemplateAction
    //{
    //    public string Type { get; set; } = "postback";
    //    public string Label { get; set; }
    //    public string Data { get; set; }
    //    public string Text { get; set; }
    //}

    //public class TemplateActionMessage : ITemplateAction
    //{
    //    public string Type { get; set; } = "message";
    //    public string Label { get; set; }
    //    public string Text { get; set; }
    //}

    //public class TemplateActionUri : ITemplateAction
    //{
    //    public string Type { get; set; } = "uri";
    //    public string Label { get; set; }
    //    public string Uri { get; set; }
    //}

    //public class TemplateActionDatetimePicker : ITemplateAction
    //{
    //    public string Type { get; set; } = "datetimepicker";
    //    public string Label { get; set; }
    //    public string Data { get; set; }
    //    public DatetimePicker Mode { get; set; }
    //    public string Initial { get; set; }
    //    public string Max { get; set; }
    //    public string Min { get; set; }
    //}

    //public enum DatetimePicker
    //{
    //    Date,
    //    Time,
    //    Datetime
    //}
    //#endregion
    //#endregion



    #region Line Profile Response Body (Receiving from LINE)
    public class Profile
    {
      /// <summary>
      /// User display name
      /// </summary>
      public string DisplayName { get; set; }
      /// <summary>
      /// User Id
      /// </summary>
      public string UserId { get; set; }
      /// <summary>
      /// Image Url
      /// </summary>
      public string PictureUrl { get; set; }
      /// <summary>
      /// Status message
      /// </summary>
      public string StatusMessage { get; set; }
    }
    #endregion
  }
}