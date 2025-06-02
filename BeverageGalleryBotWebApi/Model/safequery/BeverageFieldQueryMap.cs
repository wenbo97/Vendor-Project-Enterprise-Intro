using MongoDB.Bson;
using MongoDB.Driver;

namespace BeverageGalleryBotWebApi.Model.safequery;

public static class BeverageFieldQueryMap
{
    public static readonly Dictionary<string, Func<string, FilterDefinition<BeverageCollection>>> QueryMap =
        new()
        {
            // Full match query
            ["id"] = v => Builders<BeverageCollection>.Filter.Eq(x => x.Id, v),
            ["item_id"] = v => Builders<BeverageCollection>.Filter.Eq(x => x.ItemId, int.Parse(v)),
            ["item_name"] = v => Builders<BeverageCollection>.Filter.Eq(x => x.ItemName, v),
            ["category"] = v => Builders<BeverageCollection>.Filter.Eq(x => x.Category, v),

            // Fuzzy query
            ["item_name_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.ItemName, new BsonRegularExpression(v, "i")),
            ["mg_weight_per_box_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.MgWeightPerBox, new BsonRegularExpression(v, "i")),
            ["wet_method_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.WetMethod, new BsonRegularExpression(v, "i")),
            ["inner_bag_type_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.InnerBagType, new BsonRegularExpression(v, "i")),
            ["box_type_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.BoxType, new BsonRegularExpression(v, "i")),
            ["carton_box_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.CartonBox, new BsonRegularExpression(v, "i")),
            ["quantity_pcs_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.QuantityPcs, new BsonRegularExpression(v, "i")),
            ["category_fuzzy"] = v => Builders<BeverageCollection>.Filter.Regex(x => x.Category, new BsonRegularExpression(v, "i")),
        };
}