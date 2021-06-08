using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AStar
{
    // Start is called before the first frame update
    private GameObject origin_point;
    private LayerMask mask;
    private move_node[] open_moves;
    private Vector2[] closed_pos = { };
    private Vector2 box = new Vector2(0.7f, 0.7f);

    public AStar(GameObject origin)
    {
        origin_point = origin;
        LayerMask mask1 = LayerMask.GetMask("wall");
        LayerMask mask2 = LayerMask.GetMask("enemy");
        //LayerMask mask3 = LayerMask.GetMask("player");
        mask = mask1 | mask2;
            //| mask3;
    }

    public int get_next_move(Vector2 objective)
    {
        closed_pos = new Vector2[]{ };
        Vector2 position = new Vector2(origin_point.transform.position.x, origin_point.transform.position.y);
        open_moves = get_move_list(position, objective, 0);
        //Debug.Log(open_moves.Length);
        while(open_moves.Length > 0)
        {
            int index = get_best_move();
            move_node current_move = open_moves[index];
            if (get_heuristic(current_move.pos, objective) < 2)
            {
                return current_move.original_dir;
            }
            move_node[] new_moves = get_move_list(current_move.pos, objective, current_move.moves, current_move.original_dir);
            open_moves = open_moves.Concat(new_moves).ToArray();
            open_moves = RemoveIndex(open_moves, index);
            //Debug.Log(new_moves.Length);
        }

        return -1;
    }

    private move_node[] RemoveIndex(move_node[] IndicesArray, int RemoveAt)
    {
        move_node[] newIndicesArray = new move_node[IndicesArray.Length - 1];

        int i = 0;
        int j = 0;
        while (i < IndicesArray.Length)
        {
            if (i != RemoveAt)
            {
                newIndicesArray[j] = IndicesArray[i];
                j++;
            }

            i++;
        }

        return newIndicesArray;
    }
    private int get_best_move()
    {
        int pos = 0;
        for (int i = 0; i < open_moves.Length; i++)
        {
            if (open_moves[i].heuristic < open_moves[pos].heuristic) { pos = i; }
        }
        return pos;
    }
    private move_node[] get_move_list(Vector2 position, Vector2 objective, float current_moves, int dir = -1)
    {
        
        float current_x = position.x;
        float current_y = position.y;
        move_node[] pos_moves = { };
        move_node up = new move_node();
        up.dir = 0;
        up.pos = new Vector2(current_x, current_y + 1);
        up.heuristic = current_moves + get_heuristic(up.pos, objective);
        up.moves = current_moves + 1;
        up.original_dir = dir == -1 ? up.dir : dir;
        Collider2D col = Physics2D.OverlapBox(up.pos, box, 0, mask);
        if (col is null && !closed_pos.Contains(up.pos))
        {
            pos_moves = pos_moves.Concat(new move_node[] { up }).ToArray();
            closed_pos = closed_pos.Concat(new Vector2[] { up.pos }).ToArray();
        }

        move_node right = new move_node();
        right.dir = 1;
        right.pos = new Vector2(current_x + 1, current_y);
        right.heuristic = current_moves + get_heuristic(right.pos, objective);
        right.original_dir = dir == -1 ? right.dir : dir;
        right.moves = current_moves + 1;
        Collider2D col2 = Physics2D.OverlapBox(right.pos, box, 0, mask);
        if (col2 is null && !closed_pos.Contains(right.pos))
        {
            pos_moves = pos_moves.Concat(new move_node[] { right }).ToArray();
            closed_pos = closed_pos.Concat(new Vector2[] { right.pos }).ToArray();
        }

        move_node down = new move_node();
        down.dir = 2;
        down.pos = new Vector2(current_x, current_y - 1);
        down.heuristic = current_moves + get_heuristic(down.pos, objective);
        down.original_dir = dir == -1 ? down.dir : dir;
        down.moves = current_moves + 1;
        Collider2D col3 = Physics2D.OverlapBox(down.pos, box, 0, mask);
        if (col3 is null && !closed_pos.Contains(down.pos))
        {
            pos_moves = pos_moves.Concat(new move_node[] { down }).ToArray();
            closed_pos = closed_pos.Concat(new Vector2[] { down.pos }).ToArray();
        }

        move_node left = new move_node();
        left.dir = 3;
        left.pos = new Vector2(current_x - 1, current_y);
        left.heuristic = current_moves + get_heuristic(down.pos, objective);
        left.original_dir = dir == -1 ? left.dir : dir;
        left.moves = current_moves + 1;
        Collider2D col4 = Physics2D.OverlapBox(left.pos, box, 0, mask);
        if (col4 is null && !closed_pos.Contains(left.pos))
        {
            pos_moves = pos_moves.Concat(new move_node[] { left }).ToArray();
            closed_pos = closed_pos.Concat(new Vector2[] { left.pos }).ToArray();
        }

        return pos_moves;
    }

    private float get_heuristic(Vector2 position, Vector2 objective)
    {
        float distance_x = Mathf.Abs(objective.x - position.x);
        float distance_y = Mathf.Abs(objective.y - position.y);
        return distance_x + distance_y;
    }

}


/*
 
 // A* Search Algorithm
1.  Initialize the open list
2.  Initialize the closed list
    put the starting node on the open 
    list (you can leave its f at zero)

3.  while the open list is not empty
    a) find the node with the least f on 
       the open list, call it "q"

    b) pop q off the open list
  
    c) generate q's 8 successors and set their 
       parents to q
   
    d) for each successor
        i) if successor is the goal, stop search
          successor.g = q.g + distance between 
                              successor and q
          successor.h = distance from goal to 
          successor (This can be done using many 
          ways, we will discuss three heuristics- 
          Manhattan, Diagonal and Euclidean 
          Heuristics)
          
          successor.f = successor.g + successor.h

        ii) if a node with the same position as 
            successor is in the OPEN list which has a 
           lower f than successor, skip this successor

        iii) if a node with the same position as 
            successor  is in the CLOSED list which has
            a lower f than successor, skip this successor
            otherwise, add  the node to the open list
     end (for loop)
  
    e) push q on the closed list
    end (while loop)
 */
