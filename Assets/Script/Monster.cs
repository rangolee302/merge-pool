using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] float _top;
    [SerializeField] float _left;
    [SerializeField] float _bottom;
    [SerializeField] float _right;
    [SerializeField] float _speed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 value = Increment();
        transform.position += value * _speed;
    }

    // TODO : death function : destroy monster 

    Vector3 Increment()
    {
        Vector3 value = Vector3.zero;
        bool isTop = transform.position.z >= _top;
        bool isBottom = transform.position.z <= _bottom;
        bool isRight = transform.position.x >= _right;
        bool isLeft = transform.position.x <= _left;
        Debug.Log($"{isTop} {isBottom} {isLeft} {isRight} ");
        if ((isTop && !isRight && !isLeft) || (isTop && isLeft))
        {
            value += new Vector3(1, 0);
            return value;
        }
        if ((isBottom && !isRight && !isLeft) || (isBottom && isRight))
        {
            value += new Vector3(-1, 0);
            return value;
        }
        if ((isLeft && !isBottom && !isTop) || (isLeft && isBottom))
        {
            value += new Vector3(0, 0, 1);
            return value;
        }
        if ((isRight && !isBottom && !isTop) || (isRight && isTop))
        {
            value += new Vector3(0, 0, -1);
            return value;
        }
        return value;
    }
}
