#pragma strict

var currentFrictionValue : float;
private var skidAt : float = 1.5;
var soundEmmission : float = 50;
private var soundweight : float = 0;
var skidSound : GameObject;

function Start () {

}

function Update () {
	var hit : WheelHit;
	
	transform.GetComponent(WheelCollider).GetGroundHit(hit);
	currentFrictionValue = Mathf.Abs(hit.sidewaysSlip);
	
	if(skidAt <= currentFrictionValue && soundweight <= 0)
	{
		Instantiate(skidSound,hit.point,Quaternion.identity);
		soundweight = 1;
	}
	
	soundweight -= Time.deltaTime * soundEmmission;
}