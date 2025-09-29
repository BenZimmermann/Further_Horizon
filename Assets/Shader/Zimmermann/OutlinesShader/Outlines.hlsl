struct ScharrOperators
{
    float3x3 x;
    float3x3 y;
};
ScharrOperators GetEdgeDetectionKernels()
{
    //scharr uses 3x3 as kernels
    ScharrOperators kernels;
    kernels.x = float3x3(
        -3, -10, -3,
         0,   0,  0, 
         3,  10,  3
    );
    kernels.y = float3x3(
        -3,  0, 3, 
        -10, 0, 10,
        -3,  0, 3
    );
 //too sharp
 /*   kernels.x = float3x3(
    -2, -5, -2,
     0, 0, 0,
     2, 5, 2
);
    kernels.y = float3x3(
    -2, 0, 2,
    -5, 0, 5,
    -2, 0, 2
);
*/
    return kernels;
}
//based on the depth of the object in world
void DepthBasedOutlines_float(float2 screenUV, float2 px, out float outlines)
{
    outlines = 0;
    #if defined(UNITY_DECLARE_DEPTH_TEXTURE_INCLUDED)
    ScharrOperators kernels = GetEdgeDetectionKernels();
    float gx = 0;
    float gy = 0;
    
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            if(i == 0 && j == 0) continue;
            float2 offset = float2(i, j) * px;
            float d = SampleSceneDepth(screenUV + offset);
            gx += d * kernels.x[i + 1][j + 1];
            gy += d * kernels.y[i + 1][j + 1];
        }

    }
    float g = sqrt(gx * gx + gy * gy);
outlines = smoothstep(0.01, 0.03, g);
#endif
}
//based on the normal of the object in the world
void NormalBasedOutlines_float(float2 screenUV, float2 px, out float outlines)
{
    outlines = 0;
    #if defined(UNITY_DECLARE_NORMALS_TEXTURE_INCLUDED)
    ScharrOperators kernels = GetEdgeDetectionKernels();
    float gx = 0;
    float gy = 0;
    float3 cn = SampleSceneNormals(screenUV);
    for (int i = -1; i <= 1; i++)
    {
       for (int j = -1; j <= 1; j++)
        {
            if(i == 0 && j == 0) continue;
            float2 offset = float2(i, j) * px;
            float3 n = SampleSceneNormals(screenUV + offset);
            float dp = dot(cn, n);
            gx += dp * kernels.x[i + 1][j + 1];
            gy += dp * kernels.y[i + 1][j + 1];
        }
    }
    float g = sqrt(gx * gx + gy * gy);
    outlines = smoothstep(1, 6, g);
#endif
}