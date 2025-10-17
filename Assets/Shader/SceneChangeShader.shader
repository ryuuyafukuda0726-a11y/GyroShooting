Shader "Custom/SceneChangeShader"
{
    Properties
    {
        _HoleRadius ("Hole Radius (0-0.5)", Range(0.0, 0.5)) = 0.2 // C#から制御する穴の半径
        _Color ("Background Color", Color) = (0,0,0,1) // 黒い背景色
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // UI要素として適切に描画するための設定
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _HoleRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // UV座標を[0, 1]から[-0.5, 0.5]の範囲に変換し、中心を(0, 0)にする
                o.uv = v.uv - 0.5; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 中心(0, 0)からの距離を計算
                float distance = length(i.uv);

                // 距離が_HoleRadiusよりも小さい場合、透明にする
                // step関数を使って、距離が_HoleRadius以上なら1.0 (不透明)、そうでなければ0.0 (透明)のマスク値を得る
                float mask = step(_HoleRadius, distance); 
                
                // マスク値に応じて最終的な色を決定
                // マスクが1.0 (外側) の場合、背景色(_Color)を返す。
                // マスクが0.0 (内側) の場合、背景色のアルファを0にして透明にする。
                fixed4 finalColor = _Color;
                finalColor.a *= mask;

                return finalColor;
            }
            ENDCG
        }
    }
}