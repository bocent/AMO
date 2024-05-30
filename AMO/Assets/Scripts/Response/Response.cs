using DG.Tweening.Plugins;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int next_evolution_name;
}






#region USER DATA
[Serializable]
public class UserResponse
{
    public string status;
    public string msg;
    public string email;
    public string user_id;
    public double user_coin;
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
    public int next_evolution_id;
    public string next_evolution_name;
    public int is_used;
    public CharacterStatus status;
    public UsedAccessries accessories_used;
}

[Serializable]
public class CharacterStatus
{
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
    public string helmet_items_id;
    public string outfit_items_id;
}

[Serializable]
public class InventoryList
{
    public string[] helmet;
    public string[] outfit;
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