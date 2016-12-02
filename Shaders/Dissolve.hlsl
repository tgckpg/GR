#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SIMPLE
#define D2D_INPUT1_SIMPLE

#include "d2d1effecthelpers.hlsli"

float dissolveAmount;
float feather = 0.1;

D2D_PS_ENTRY( main )
{
	float4 color = D2DGetInput( 0 );
	float mask = D2DGetInput( 1 ).r;

	float adjustedMask = mask * ( 1 - feather ) + feather / 2;

	float alpha = saturate( ( adjustedMask - dissolveAmount ) / feather + 0.5 );

	return color * alpha;
}