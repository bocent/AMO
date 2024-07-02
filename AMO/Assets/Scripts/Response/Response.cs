using DG.Tweening.Plugins;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Response
{
    public string status;
    public string msg;
}


[Serializable]
public class LoginResponse
{
    public string status;
    public string msg;
    public string email;
    public string uid;
    public string token;
}


[Serializable]
public class RegistrationResponse
{
    public string status;
    public string msg;
    public string email;
}


[Serializable]
public class CharacterResponse
{
    public string status;
    public string msg;
    public CharacterData[] karakter;
}

[Serializable]
public class CharacterData
{
    public int karakter_id;
    public string nama_karakter;
    public string nama_spesies;
    public string desc;
    public string price;
    public EvolutionData[] evolution;
}

[Serializable]
public class EvolutionData
{
    public int evolution_id;
    public string evolution_name;
    public int required_level;
    public int end_level;
    public int experience_to_evolution;
    public int next_evolution_id;
    public string next_evolution_name;
}






#region USER DATA
[Serializable]
public class UserResponse
{
    public string status;
    public string msg;
    public string name;
    public string email;
    public string user_id;
    public long user_coin;
    public string waktu_server;
    public ChargeItems charge_items;
    public List<UserCharacter> karakter_user;
    public InventoryList inventory;
}

[Serializable]
public class ChargeItems
{
    public int energy_charge_stock;
    public int energy_super_charge_stock;
    public int fix_charge_stock;
}

[Serializable]
public class UserCharacter
{
    public int karakter_id;
    public string nama_karakter;
    public int level;
    public int experience;
    public int evolution_id;
    public string evolution_name;
    public string experience_to_evolution;
    public int level_to_next_evolution;
    public int? next_evolution_id;
    public string next_evolution_name;
    public int is_used;
    public CharacterStatus status;
    public NeedAction need_action;
    public UsedAccessries accessories_used;
}

[Serializable]
public class NeedAction
{
    public int need_clean;
    public int need_charge;
    public int need_repair;
}

[Serializable]
public class CharacterStatus
{
    public int karakter_id;
    public int hunger;
    public int happiness;
    public int health;
    public int energy;
    public string last_fed;
    public string last_played;
    public string last_medicate;
}

[Serializable]
public class UsedAccessries
{
    public int helmet_items_id;
    public int outfit_items_id;
}

[Serializable]
public class InventoryList
{
    public ItemData[] helmet;
    public ItemData[] outfit;
}

[Serializable]
public class ItemData
{
    public int user_invenrtory_id;
    public int items_id;
    public string item_name;
    public string kategori;
    public int karakter_id;
    public string karakter_name;
    public int evolution_id;
    public string evolution_name;
    public int is_used;
}

#endregion


[Serializable]
public class ExperienceResponse
{
    public string status;
    public string msg;
    public int new_experience;
    public bool evolution_up;
    public string new_evolution_id;
    public string new_evolution_name;
}


#region SHOP_RESPONSE
[Serializable]
public class ShopResponse
{
    public string status;
    public string msg;
    public ShopList item_sell;
}

[Serializable]
public class ShopList
{
    public ShopItem[] karakter;
    public ShopItem[] charge;
    public ShopAccessory accessories;
}

[Serializable]
public class ShopItem
{
    public int item_sell_id;
    public int items_id;
    public string item_name;
    public string kategori;
    public string karakter_id;
    public string karakter_name;
    public string evolution_id;
    public string evolution_name;
    public int qty;
    public int price;
}

[Serializable]
public class ShopAccessory
{
    public ShopItem[] helmet;
    public ShopItem[] outfit;
}

#endregion

[Serializable]
public class UseItemResponse
{
    public string status;
    public string msg;
    public string charge_item;
    public int sisa_stok;
}


[Serializable]
public class ItemResponse
{
    public string status;
    public string msg;
    public IventoryItem items;
}

[Serializable]
public class IventoryItem
{
    public ItemData[] charge;
    public InventoryList accessories;
}











[Serializable]
public class BuyCoinResponse
{
    public string status;
    public string msg;
    public string trx_id;
    public string payment_url;
}

[Serializable]
public class BuyItemResponse
{
    public string status;
    public string msg;
    public int sisa_coin;
}



[Serializable]
public class CoinListResponse
{
    public string status;
    public string msg;
    public ItemCoin[] items_coin;
}

[Serializable]
public class ItemCoin
{
    public int topup_coin_id;
    public string name;
    public int qty;
    public string price;
    public string desc;
}




[Serializable]
public class CharacterStatusResponse
{
    public string status;
    public string msg;
    public CharacterStatus status_karakter;
    public NeedAction need_action;
}



[Serializable]
public class ScanCardResponse
{
    public string status;
    public string msg;
    public CardReward items_on_card;
    public string[] user_gets;
}

[Serializable]
public class CardReward
{
    public int coins;
    public string[] karakter;
    public string[] items;
    public string[] charge_item;
}

[Serializable]
public class AlarmResponse
{
    public string status;
    public string msg;
    public Alarm[] alarm;
}

[Serializable]
public class Alarm
{
    public int alarm_id;
    public string hour;
    public string minute;
    public int sun;
    public int mon;
    public int tue;
    public int wed;
    public int thu;
    public int fri;
    public int sat;
    public string title;
    public int active;
}