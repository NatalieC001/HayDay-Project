﻿using UnityEngine;
using System.Collections;

namespace HayDay
{
	public class Bidder : MonoBehaviour
	{
	    protected float cash;
	    protected float interest;
	    protected float desiredPrice;
	    protected float bidStartTime;
	    protected float bidWaitTime;
	    protected bool bid;

	    void Start()
	    {
	        cash = 40000;
	    }

	    void FixedUpdate()
	    {
			// Bidding on cow if within desired price range
	        if (bid && Time.time > bidStartTime + bidWaitTime)
	        {
	            UIMart.BidOnCow(this, desiredPrice);  
	            Animator animator = GetComponent<Animator>();
	            animator.SetTrigger("Bid");
	            StartCoroutine(SpawnBidSprite(transform.position));
	        }
	    }

		public void ConsiderBidding(Cow cow, int currentPrice)
	    {
	        if (currentPrice > cash)
	            return;

	        bidStartTime = Time.time;
	        desiredPrice = currentPrice + Random.Range(500, 800);
	        bidWaitTime = Random.Range(5, 12);

	        switch (cow.breed)
	        {
	            case "Angus":
	                interest = 1f;
	                break;
	            case "Brangus":
	                interest = .5f;
	                break;
	            case "Charolais":
	                interest = .75f;
	                break;
	            case "Hereford":
	                interest = .85f;
	                break;
	            case "Holstein Friesian":
	                interest = .65f;
	                break;
	            case "Shorthorn":
	                interest = .35f;
	                break;
	        }
			
	        bidWaitTime *= interest;

	        if (desiredPrice < cash)
	        {
	            bid = true;
	        }
	    }

	    private IEnumerator SpawnBidSprite(Vector3 position)
	    {
	        GameObject bidSprite = Instantiate(Resources.Load("Bid!") as GameObject);
	        position.y += 5;
	        bidSprite.transform.position = position;
	 
	        yield return new WaitForSeconds(1f);

	        Destroy(bidSprite);

	    }

	    public void StopBidding()
	    {
	        bid = false;
	    }
	}
}