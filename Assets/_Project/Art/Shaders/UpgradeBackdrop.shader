Shader "StackAttack/UI/UpgradeBackdrop"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _TopColor ("Top Color", Color) = (0.388, 0.776, 0.980, 1)
        _BottomColor ("Bottom Color", Color) = (0.561, 0.196, 0.969, 1)

        _StripeColor ("Stripe Color", Color) = (1, 1, 1, 0.09)
        _StripeScale ("Stripe Scale", Float) = 9
        _StripeSpeed ("Stripe Speed", Float) = 0.05

        _TileColor ("Tile Color", Color) = (1, 1, 1, 0.08)
        _TileScale ("Tile Scale", Float) = 7
        _TileSpeed ("Tile Speed", Float) = 0.035
        _TileRound ("Tile Roundness", Range(0.01, 0.4)) = 0.14

        _Phase ("Phase", Float) = 0
        _Aspect ("Aspect", Float) = 0.5625

        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _TopColor;
            fixed4 _BottomColor;
            fixed4 _StripeColor;
            fixed4 _TileColor;
            float _StripeScale;
            float _StripeSpeed;
            float _TileScale;
            float _TileSpeed;
            float _TileRound;
            float _Phase;
            float _Aspect;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            float RoundedBox(float2 p, float2 halfSize, float radius)
            {
                float2 d = abs(p) - halfSize + radius;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - radius;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord;

                fixed4 col = lerp(_BottomColor, _TopColor, saturate(uv.y));

                // Diagonal bands sliding across the upper half.
                float diagonal = (uv.x * _Aspect + uv.y) * _StripeScale + _Phase * _StripeSpeed;
                float band = abs(frac(diagonal) - 0.5) * 2.0;
                float stripe = 1.0 - smoothstep(0.25, 0.75, band);
                float upper = smoothstep(0.2, 0.9, uv.y);
                col.rgb = lerp(col.rgb, _StripeColor.rgb, stripe * _StripeColor.a * upper);

                // Rounded tiles rising through the lower half. Scaling x by the rect
                // aspect is what keeps a tile square rather than stretched.
                float2 grid = float2(uv.x * _Aspect, uv.y) * _TileScale;
                grid.y -= _Phase * _TileSpeed;
                float2 cell = frac(grid) - 0.5;
                float tileDistance = RoundedBox(cell, float2(0.34, 0.34), _TileRound);
                float tile = 1.0 - smoothstep(-0.015, 0.015, tileDistance);
                float lower = 1.0 - smoothstep(0.1, 0.75, uv.y);
                col.rgb = lerp(col.rgb, _TileColor.rgb, tile * _TileColor.a * lower);

                col.rgb *= i.color.rgb;
                col.a = i.color.a;
                return col;
            }
            ENDCG
        }
    }
}
