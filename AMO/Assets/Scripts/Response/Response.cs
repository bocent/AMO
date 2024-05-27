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
    public string email;
    public string user_id;
    public double user_coin;
    public List<UserCharacter> karakter_user;
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
    public int next_evolution_id;
    public string next_evolution_name;
    public int is_last_used;
    public CharacterStatus status;
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
#endregion