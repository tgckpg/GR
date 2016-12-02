#define D2D_INPUT_COUNT 0
#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"

float2 center;
float t1;
float t2;
float dpi = 96;

D2D_PS_ENTRY( main )
{
	float2 positionInPixels = D2DGetScenePosition().xy;

	float2 positionInDips = positionInPixels * 96 / dpi;

	float d = distance( center, positionInDips ) * 0.05 + ( 12 - 24.7 * t1 ) + 10;
	float v = ( d == 0 ? 1 : ( sin( d ) / d ) ) * 100 - ( 5 + 8 * t2 );

	return float4( v, v, v, 1 );
}