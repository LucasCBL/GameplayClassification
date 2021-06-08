using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ActionBranch
{
    public int random_move_prob;
    public int move_towards_enemy_prob;
    public int move_towards_goal_prob;
    public int run_away_prob;
    public int mov_coin_prob;
    public int range_attack_prob;
    public int coin_range = 0;
    public int coins_detect = 0;
    public int enemy_detect = 0;
    public int health_min = 0;
    public int health_max = 0;
    public int enemy_min = 0;
    public int enemy_max = 0;
    public int goal_max = 0;
    public int max_enemy_health = 0;

    public bool check_conditions(int health, float enemy_distance, int enemy_health, float goal_distance, float coin_distance, bool is_coin, bool is_enemy)
    {
        if(( health > health_min || health_min == 0) && (health < health_max || health_max == 0) && (enemy_distance > enemy_min || enemy_min == 0))
        {
            if ((enemy_distance < enemy_max || enemy_max == 0) && (goal_distance < goal_max || goal_max == 0) && (enemy_health < max_enemy_health|| max_enemy_health == 0))
            {
                if ((coin_distance < coin_range || coin_range == 0) && (is_coin || coins_detect == 0) && (is_enemy || enemy_detect == 0))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    
    public int get_action()
    {
        System.Random r = new System.Random();
        int total = random_move_prob + move_towards_enemy_prob + move_towards_goal_prob + mov_coin_prob + range_attack_prob + run_away_prob;
        int rInt = r.Next(0, total);

        int current_sum = random_move_prob;
        if(rInt < current_sum)
        {
            return 0;
        }

        current_sum += move_towards_enemy_prob;
        if (rInt < current_sum)
        {
            return 1;
        }

        current_sum += move_towards_goal_prob;
        if (rInt < current_sum)
        {
            return 2;
        }

        current_sum += run_away_prob;
        if (rInt < current_sum)
        {
            return 3;
        }

        current_sum += mov_coin_prob;
        if (rInt < current_sum)
        {
            return 4;
        }

        current_sum += range_attack_prob;
        if (rInt < current_sum)
        {
            return 5;
        }

        return 0;

    }
}
