using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BeverageGalleryBotWebApi.Model;

/// <summary>
/// Beverage base key and value
/// </summary>
public class BeverageCollection
{
    /// <summary>
    /// Mongodb object id
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("item_id")] public int ItemId { get; set; } = int.MinValue;

    [BsonElement("item_name")] public string ItemName { get; set; } = string.Empty;

    [BsonElement("image_id")] public string ImageId { get; set; } = string.Empty;

    [BsonElement("mg_weight_per_box")] public string MgWeightPerBox { get; set; } = string.Empty;

    [BsonElement("wet_method")] public string WetMethod { get; set; } = string.Empty;

    [BsonElement("inner_bag_type")] public string InnerBagType { get; set; } = string.Empty;

    [BsonElement("box_type")] public string BoxType { get; set; } = string.Empty;

    [BsonElement("carton_box")] public string CartonBox { get; set; } = string.Empty;

    [BsonElement("quantity_pcs")] public string QuantityPcs { get; set; } = string.Empty;

    [BsonElement("category")] public string Category { get; set; } = string.Empty;
}