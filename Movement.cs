using System.Collections;
using System.Collections.Generic;
using UnityEngine;

        
public class Movement : MonoBehaviour {
		
		
		public bool jumpUsed=false;
		public bool hitDown=false;
		public int hitJump=0;
		public float walkspeed=17;
		public float walkspeedmax=40; //possibly obsolete, will fix later.
		public float wspeed=7;
		public float rollspeed=7;
        public float xvel=0; //float x velocity
        public float yvel=0; // float y velocity
		private float direction=0;
        private Animator anim; // private because we dont need to access it in the main editor.
        public bool facing_right=true; //self explanatory name . png
		public bool grounded=false;
	
        private SpriteRenderer spriteRenderer;
    // Use this for initialization
    void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>(); //sprite renderer (wonder if I can make it change color)
        anim = this.GetComponent<Animator>(); // anim = the animator component on the object the script is on.        
    }
    

    
	
	void OnCollisionStay2D(Collision2D coll)
    {
		//onwall=false;
		
		if (coll.contacts[0].otherCollider.transform.gameObject.name == "DetectiveIdle_0"){             

			foreach(ContactPoint2D contact in coll.contacts)							//honestly i dont know if this does anything
			{
					
				anim.SetBool("onground", true);
				grounded=true;
				yvel=0;

			}
		}
		if (coll.contacts[0].otherCollider.transform.gameObject.name == "leftcollider")	//This is back side wall collision.
		{
			if(spriteRenderer.flipX==false && xvel < 0){
				xvel=xvel/2;
			}
			if(spriteRenderer.flipX==true && xvel > 0){
				xvel=xvel/2;
			}
		}
		if (coll.contacts[0].otherCollider.transform.gameObject.name == "rightcollider") //This is front side wall collision
		{
			if(spriteRenderer.flipX==false && xvel > 0){
				xvel=xvel/2;
			}
			if(spriteRenderer.flipX==true && xvel < 0){
				xvel=xvel/2;				
			}
			
		}
		if (coll.contacts[0].otherCollider.transform.gameObject.name == "ceilcollider")
		{
			yvel=-yvel/2;					//Bouncy ceilings!
		}
    }

	void OnCollisionExit2D(Collision2D coll)
    {
		if(yvel!=0){
			grounded=false;					//If you leave collision, you aren't on the ground according to the code.
            anim.SetBool("onground", false);//Fixed the bug with multiple collision spots
		}else{
			yvel-=0.05f;					//This is something I'm using for collision testing.
		}

    }
	
	
    //FixedUpdate is called at a fixed time per second. (0.02)
    void FixedUpdate (){
		
		
		if(Input.GetKeyUp(KeyCode.UpArrow)){
			jumpUsed=false; 				//This detects if you tapped up since last time you jumped.
		}									//This is so you can't hold space to jump.
		
		if(hitJump>0){						//This was set to an int rather than a bool so that
			hitJump=hitJump-1;				//there could be a frame window for jumps.
		}
		if(Input.GetKeyDown(KeyCode.UpArrow)&&jumpUsed==false){
		hitJump=1;							//However this is useless now because I set the frame window to 0
											//by making this set to 1. but changing it back to a bool might
		}									//cause some errors with the code so i'll leave it for now.

		
		if (Input.GetAxis("Vertical")<-0.5){//Hacky way to test if you're holding the Down key.
			hitDown=true;					//should replace soon tbh
		}else{
			hitDown=false;					//If you're holding down, holdingdown equals true. If not, you're not.
		}
		anim.SetBool("holdingdown",hitDown);//Sets the animation variable to whatever above to save a line of code
		
		
		direction=Input.GetAxis("Horizontal");		//Checks what direction your stick is going. This will be obsolete later when I update to GetKeyDown.
		if(anim.GetBool("onground")==true){			//HAHAHAHA WHAT A PLEB CHECKING ANIMATION VARIABLES RATHER THAN THE VARIABLE grounded
			yvel-=0.05f;							//Another thing similar to the one in the exit function for testing collision.
			if(hitDown==true&&grounded==true){		//If you're rolling, gotta go fast
				wspeed=walkspeed+rollspeed;			//rolling speed + normal speed = fast speed
			}else{
				wspeed=walkspeed;					//normal speed = slow speed
			}
		if(direction>0.1){							//Hacky, the sequel. Replace with GetKeyDown later.
			
			xvel=(xvel+(wspeed)*2)/3;				//Make him go that speed, but dont cut to it like a robot.
		
			if(facing_right==false){				//If you're looking left and moving right, gotta flip around!
				facing_right=true;
				spriteRenderer.flipX = false;
			}

		}else if(direction<-0.1){					//insert obligatory "replace with GetKeyDown" message here
			xvel=(xvel+(-wspeed)*2)/3;
		
			if(facing_right==true){					//If you're looking right and moving left, ditto!
				facing_right=false;
				spriteRenderer.flipX = true;
			}
		
		}else{										//If you aint moving, you stop. Slides you to a halt rather quickly!
			xvel=xvel/2;
		}
		
		
		if(direction!=0){							//If you are not not moving (hehe) then 
			anim.SetBool("running", true);			//running is true.
		}else{										//If you are not moving, then
			anim.SetBool("running", false);			//running is false.
		}
		if(Mathf.Abs(xvel)<0.1){					//If you're barely moving 
			xvel=0;									//then meh its close enough to zero :kappa:
		}											//(why did i write kappa in c#)

		if (hitJump>0){								//If you hit jump, weird non-bool version
			jumpUsed=true;							//Then you jumped so let go of the jump button...
			hitJump=0;								//Easy 100% works fix to prevent weird jumping
            yvel=12;								//Need to replace 12 with a public float jumpheight=12;
			grounded=false;							//You ain't on the ground if you jumped :D
			anim.SetBool("onground", false);		//Animation needs to know that
			transform.position += Vector3.up/5;		//This is so you aren't colliding with the ground the next frame.
			}
        }
													
		if(anim.GetBool("onground")==false){		//If you are not touching ground
		
			xvel=(xvel*2+(direction*walkspeed)*3)/5;//This is in a strange place...but it works as intended...might have to look into this...
													//Intended to make air steering slightly slower than walking.
		
			anim.SetBool("upwards", true);				//Easier than an else at the end.
			if(yvel<=0.5f){								//If you are going down...
				yvel-=0.1f;									//Then fall faster...
				anim.SetBool("upwards", false);				//and you aren't going up.
			}
			if(hitDown){								//If you're holding down while midair..
				yvel-=0.4f;								//You dive! Needs to be set to a public float divespeed
			}
			yvel-=0.4f;									//Plain gravity. Needs to be set to a public float gravity
			if(yvel<-20){								//If you're falling hella fast...
			
				yvel=(yvel-20)/2;						//Slow down, man!
			
			}
			if(xvel<-walkspeedmax){						//If you walking fast...
				xvel=(xvel-walkspeedmax)/2;				//Slow down!
			}
			if(xvel>walkspeedmax){						//This bit of code is more of a just in case
				xvel=(xvel+walkspeedmax)/2;				//since walkspeed + rollspeed don't provide sufficient speed to reach here.
			}
        //transform.position += Vector3.up * yvel * Time.deltaTime;  // position = itself + vector3.up x y velocity x framerate
		}												//whew finally the end of that bunch of code!
        //end if
		transform.position += (Vector3.up * yvel + Vector3.right * xvel) * Time.deltaTime;		//Move him!
    }
}