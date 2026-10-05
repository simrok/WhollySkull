Shader "keebar/lightning2"
{
    Properties
    {
        [Header(Globle)]
		_LightningColor ("Color",Color) = (1.0,0.698039,0.2)
		_Sharpness("Glowing Sharpness",Range(0,10)) = 1.9
		_Power("Intensity",Range(0.0001,0.1)) = 0.015 
		_FadeStart("Fade Start",Range(0,10)) = 9
		_FadeLength("Fade Length",Range(0,10)) = 1
		_Speed("Speed",range(0,100)) = 1
		_RandomWithPos ("Random by Position", Range(0,1)) = 0
		_RandowWithAngle ("Random by Rotation", Range(0,1)) = 0
		[KeywordEnum(Off, On)] _LockTarget ("Lock On Target", Float) = 1
		_GlobalAlpha("GlobalAlpha",Range(0.1,1.0)) = 1.0
		
		[Header(Trunk)]
		_Seedcale ("Low Scale", Float) = 2.5
        _Seedpeed ("Low Frequency", Float) = 0.2
		_Strength ("Low Amplitude", Float) = 0.3
		_Seedcale1 ("High Scale", Float) = 15.0
        _Seedpeed1 ("High Frequency", Float) = 0.2
		_Strength1 ("High Amplitude", Float) = 0.015
		[Toggle(TRUNK_DETAIL)]_TRUNK_DETAIL("Trunk Detail",int) = 1
		
		[Header(Branch)]
		_BranchFadeStart ("Fade Start",Range(0,10)) = 0.5
		_BranchFadeLength("Fade Length",Range(0,10)) = 1.7
		_BranchMoveSpeed("Move Speed", Range(0,2)) = 0.1
		_BranchLowScale("Low Scale",Float) = 0.5
		_BranchLowFrequency("Low Frequency", Range(0,10)) = 3
		_BranchLowStrength("Low Amplitude",Range(0,10)) = 2.5
		_BranchHighScale("High Scale",Float) = 35.0
		_BranchHighFrequency("High Frequency",Range(0,10)) = 1.5
		_BranchHighStrength("High Amplitude",Range(0,10)) = 0.1
		_MaxBranches("Max Number of Branches",Range(0,10)) = 5
		_BranchPropability("Probability of Branch",Range(0,1)) = 0.8
		_BranchDuration("Duration of Branch",Range(0.1,100)) = 15
		
		[Enum(UnityEngine.Rendering.BlendMode)] _ScrBlend1 ("_ScrBlend1", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend1 ("_DstBlend1", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        Pass
        {
            BlendOp add
			Blend [_ScrBlend1] [_DstBlend1]
			Cull off
			Zwrite off
			
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature TRUNK_DETAIL
			#pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 posAngleSeed : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float _Seedcale;
            float _Seedpeed;
			float _Strength;
			//float _Seed;
			
			float _Seedcale1;
            float _Seedpeed1;
			float _Strength1;
			//float _Seed1;
			
			float _BranchFadeStart;
			float _BranchFadeLength;
			float _BranchMoveSpeed;
			float _BranchHighScale;
			float _BranchLowScale;
			float _BranchLowFrequency;
			float _BranchHighFrequency;
			float _BranchLowStrength;
			float _BranchHighStrength;
			int _MaxBranches;
			float _BranchPropability;
			float _BranchDuration;
			
			fixed3 _LightningColor;
			float _Sharpness;
			float _Power;
			float _FadeStart;
			float _FadeLength;
			float _Speed;
			float _RandomWithPos;
			float _RandowWithAngle;
			uint _LockTarget;
			float _GlobalAlpha;

			
			uint pcg_hash(uint seed)
			{
				// 1. 状态转换（线性同余 LCG 步骤）
				// 使用大质数增加数据的混乱度
				uint state = seed * 747796405u + 2891336453u;
				
				// 2. 置换输出（PCG 的核心：通过位移和异或进一步打乱）
				uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
				uint result = (word >> 22u) ^ word;
    
				return result;
			}
			
			float pcg_hash01(uint seed)
			{
				return pcg_hash(seed)/4294967295.0;
			}
			
			float2 pcg_hash22(float2 p)
			{
				// 将 float2 转换为 uint2 种子
				uint2 v = asuint(p);
				
				// 分别对 X 和 Y 进行 hash，并组合
				uint hx = pcg_hash(v.x ^ pcg_hash(v.y));
				uint hy = pcg_hash(v.y ^ pcg_hash(hx));
				
				// 将 uint 映射回 [-1.0, 1.0] 的 float2 梯度向量
				return -1.0 + 2.0 * float2(hx, hy) / 4294967295.0;
			}

            // Standard Perlin Noise core
            float perlin_noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

                return lerp(lerp(dot(pcg_hash22(i + float2(0,0)), f - float2(0,0)), 
                                 dot(pcg_hash22(i + float2(1,0)), f - float2(1,0)), u.x),
                            lerp(dot(pcg_hash22(i + float2(0,1)), f - float2(0,1)), 
                                 dot(pcg_hash22(i + float2(1,1)), f - float2(1,1)), u.x), u.y);
            }
			
			// Helper to get a random float2 gradient in range [-1, 1]
            float2 get_gradient(float2 p) {
                uint2 v = asuint(p);
                uint h = pcg_hash(v.x ^ pcg_hash(v.y));
                // Convert hash to angle for more uniform gradients
                float angle = h * (2.0 * 3.14159265 / 4294967295.0);
                return float2(cos(angle), sin(angle));
            }

            // 2. Core 2D Simplex Noise
            float simplex_noise(float2 v) {
                const float K1 = 0.366025404; // (sqrt(3)-1)/2
                const float K2 = 0.211324865; // (3-sqrt(3))/6

                // Skew the input space to find which "rhombus" cell we're in
                float2 i = floor(v + (v.x + v.y) * K1);
                
                // Unskew back to find the distance from the first vertex
                float2 x0 = v - (i - (i.x + i.y) * K2);

                // Determine which triangle of the rhombus we are in
                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                
                // Offsets for the other two vertices
                float2 x1 = x0 - i1 + K2;
                float2 x2 = x0 - 1.0 + 2.0 * K2;

                // Calculate radial contribution (circular kernels)
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
                float3 n = m * m * m * m * float3(dot(x0, get_gradient(i)), 
                                                  dot(x1, get_gradient(i + i1)), 
                                                  dot(x2, get_gradient(i + 1.0)));

                // Scale the result to [-1, 1]
                return dot(n, float3(70.0, 70.0, 70.0));
            }
            

            v2f vert (appdata v) {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				float posSeed = pcg_hash01((uint)dot(float3(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3]),
								float3(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3])*10037));
				uint rotationHash0 = asuint(unity_ObjectToWorld[0][0])^(asuint(unity_ObjectToWorld[0][1])<<2)^(asuint(unity_ObjectToWorld[0][2])>>2);
				uint rotationHash1 = asuint(unity_ObjectToWorld[1][0])^(asuint(unity_ObjectToWorld[1][1])<<2)^(asuint(unity_ObjectToWorld[1][2])>>2);
				uint rotationHash2 = asuint(unity_ObjectToWorld[2][0])^(asuint(unity_ObjectToWorld[2][1])<<2)^(asuint(unity_ObjectToWorld[2][2])>>2);
				float angleSeed = pcg_hash01(rotationHash0^(rotationHash1<<2)^(rotationHash2>>2));
				
				o.posAngleSeed = float2(posSeed*_RandomWithPos,angleSeed*_RandowWithAngle);
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float time = _Time.y*_Speed;
				//generate trunk
				float2 p = float2(i.uv.x , time * _Seedpeed) * _Seedcale + i.posAngleSeed;
				#if defined(TRUNK_DETAIL)
					float2 p1 = float2(i.uv.x , time * _Seedpeed) * _Seedcale*2.0f + float2(31.41, 58.26) + i.posAngleSeed;
				#endif
				float2 p2 = float2(i.uv.x , time * _Seedpeed1) * _Seedcale1 + float2(61.23, 18.66) + i.posAngleSeed;
				#if defined(TRUNK_DETAIL)
					float2 p3 = float2(i.uv.x , time * _Seedpeed1) * _Seedcale1*2.0f + float2(38.39, 46.76) + i.posAngleSeed;
				#endif
				//float2 p4 = float2(i.uv.x , time * _Seedpeed) * _Seedcale + float2(52.33, 76.26) + i.posAngleSeed;
				//float2 p5 = float2(i.uv.x , time * _Seedpeed2) * _Seedcale2*2.0f + float2(18.31, 36.77) + i.posAngleSeed;
				float amplitudeLimiter = (1.0 - abs(i.uv.x - 0.5)*2.0)*_LockTarget + i.uv.x*(1 - _LockTarget);
				#if defined(TRUNK_DETAIL)
					float trunkY = lerp(0.5,simplex_noise(p)*_Strength + simplex_noise(p1)*_Strength*0.5 
									+ simplex_noise(p2)*_Strength1 + simplex_noise(p3)*_Strength1*0.5 
									+ 0.5,pow(max(0.0,amplitudeLimiter),0.5));
				#else
					float trunkY = lerp(0.5,simplex_noise(p)*_Strength + simplex_noise(p2)*_Strength1 + 0.5,pow(max(0.0,amplitudeLimiter),0.5));
				#endif
				float trunkIntensity = pow(1/abs(i.uv.y - trunkY)*_Power,_Sharpness);
				
				//generate branches
				float branchIntensity = 0;
				for(int j = 0; j < _MaxBranches; j++)
				{
					float2 bp = float2(i.uv.x,(time + j) * _Seedpeed*_BranchLowFrequency) *_BranchLowScale + i.posAngleSeed;
					float2 bp1 = float2(i.uv.x,(time + j) * _Seedpeed1*_BranchHighFrequency) *_BranchHighScale + float2(31.41, 58.26) + i.posAngleSeed;
					float2 bp2 = float2(j,time*_BranchMoveSpeed)*2 + float2(-42.73, 12.98) + i.posAngleSeed;
					fixed visibility = step(pcg_hash01(j + floor((time + dot(i.posAngleSeed,i.posAngleSeed))*_BranchDuration)),_BranchPropability);
					float branchY = (simplex_noise(bp)*_BranchLowStrength + simplex_noise(bp1)*_BranchHighStrength)*0.5 + 0.5;
					float branchX = simplex_noise(bp2)*0.5 + 0.5;
					branchY = lerp(trunkY,branchY,smoothstep(branchX,branchX + 0.25,i.uv.x));
					float distanceX = abs(i.uv.x - branchX)*10;
					float randomLength = pcg_hash01(j*3 + floor((time + dot(i.posAngleSeed,i.posAngleSeed))*_BranchDuration));
					branchIntensity += lerp(pow(1/abs(i.uv.y - branchY)*_Power,_Sharpness),0,
								smoothstep(_BranchFadeStart*randomLength,_BranchFadeStart + _BranchFadeLength*randomLength,distanceX))*visibility;
				}
				
				float intensity = lerp(trunkIntensity + branchIntensity,0,smoothstep(_FadeStart,_FadeStart + _FadeLength,i.uv.x*10));
				
				return saturate(fixed4(_LightningColor * intensity, intensity))*_GlobalAlpha;
            }
            ENDCG
        }
    }
}