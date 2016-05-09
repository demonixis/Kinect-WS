// Kinect data
let bodies = null;

function startDemo() {
    // ***************
    // * Scene setup *
    // ***************
    let kinectJoints = [];
    let canvas = document.getElementById('renderCanvas');
    let engine = new BABYLON.Engine(canvas, true);
    let scene = new BABYLON.Scene(engine);
    let mainCamera = new BABYLON.FreemainCamera("MainCamera", new BABYLON.Vector3(0, 2, -2), scene);
    mainCamera.setTarget(BABYLON.Vector3.Zero());
    mainCamera.attachControl(canvas, true);

    let mainLight = new BABYLON.HemisphericmainLight("mainLight1", new BABYLON.Vector3(0, 1, 0), scene);
    mainLight.intensity = 0.7;


    // ********************
    // * Creating objects *
    // ********************
    let skybox = BABYLON.Mesh.CreateBox("skybox", 250, scene);
    skybox.material = new BABYLON.StandardMaterial("skyboxMaterial", scene);
    skybox.material.backFaceCulling = false;
    skybox.material.reflectionTexture = new BABYLON.CubeTexture("Assets/Textures/skybox/skybox", scene);
    skybox.material.reflectionTexture.coordinatesMode = BABYLON.Texture.SKYBOX_MODE;
    mainLight.excludedMeshes.push(skybox);

    let ground = BABYLON.Mesh.CreateGround("ground1", 6, 6, 2, scene);
    ground.material = new BABYLON.StandardMaterial("groundMat", scene);
    ground.material.diffuseTexture = new BABYLON.Texture("Assets/Textures/ground.png", scene);


    // **************************
    // * Creating Kinect Avatar *
    // **************************
    let jointParent = new BABYLON.Mesh("KinectAvatar", scene);
    jointParent.isVisible = false;
    jointParent.position.y += 1;

    let jointMaterial = new BABYLON.StandardMaterial("JointMaterial", scene);
    jointMaterial.diffuseTexture = new BABYLON.Texture("Assets/Textures/box.png", scene);

    for (let i = 0; i < 25; i++) {
        kinectJoints.push(BABYLON.Mesh.CreateSphere("sphere_" + i, 16, 0.25, scene));
        kinectJoints[i].jointType = i;
        kinectJoints[i].position.y = i * 0.2;
        kinectJoints[i].scaling = new BABYLON.Vector3(0.4, 0.4, 0.4);
        kinectJoints[i].parent = jointParent;
        kinectJoints[i].material = jointMaterial;
    }

    // Start the engine
    engine.runRenderLoop(function() {
        if (bodies != null && bodies.length) {
            let body = bodies[0];
            for (let i = 0; i < 25; i++) {
                let joint = body.kinectJoints[kinectJoints[i].jointType];
                kinectJoints[i].position.copyFromFloats(joint.Position.x, joint.Position.y, joint.Position.z);
            }
        }
        scene.render();
    });

    window.addEventListener('resize', function() {
        engine.resize();
    });
}

window.onload = function (event) {
    // Connection to the Kinect Server.
    let ws = new WebSocket("ws://127.0.0.1:8181");
    ws.onopen = (e) => console.log("Connected to server");
    ws.onerror = (e) => console.log("Error detected");
    ws.onmessage = (e) => {
        bodies = JSON.parse(e.data);
    };

    // Start the demo.
    startDemo();
};