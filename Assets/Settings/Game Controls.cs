// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Game Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Game Controls"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""c5ae2f2a-f044-4182-8bf5-7713aa0d2efd"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""089cc41e-92f3-44c0-ad0d-c8bba80aa357"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""New action1"",
                    ""type"": ""Button"",
                    ""id"": ""599a95e1-10e7-4e40-8de6-7c05af813afb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5f55db12-0ca7-424d-9e08-f3ee3cb63154"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b476ef46-fb01-4c79-9923-761b065ac828"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""New action1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""In Game"",
            ""id"": ""0e7d3c8e-9222-4efc-bbaf-b34a099b42b4"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Value"",
                    ""id"": ""2fdec454-8641-4e3b-87be-e4775babe78b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""d5ddfa9a-4fd1-43ea-97d5-105d81e2c10a"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Button"",
                    ""id"": ""837ba05d-5d93-47df-8121-97df2ab5a941"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Open Menu"",
                    ""type"": ""Button"",
                    ""id"": ""6db0e69b-6355-4a98-a66d-f0665257498c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Gyro"",
                    ""type"": ""Value"",
                    ""id"": ""aef604d6-87fd-447f-953f-dac3582a64b9"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b657d68e-7d5b-4e98-aa24-dd22b365fde6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73fa396e-7756-48bb-9683-a9220d5dbbae"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e65cc2c6-5723-4b13-92bd-7626d0d2a8a3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""0bc1770c-a1da-458a-a1e3-8cc06ae781b3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""08330b7d-9f08-42a1-b7fd-0df6be1cac86"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e1e18365-6218-47f2-9ad8-d5d221f0986f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ca1ffeac-38a9-41ab-9a22-bc0c7cf5fe48"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ca2c949c-b677-4aef-9e60-009204799dfa"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""00c89d32-d91e-4f30-a7a8-0c00ef92ee70"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f8d93955-6851-47c3-aa53-8135f1831b44"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b8cf813a-f341-465e-ab95-2d035b379c60"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26b7ed71-a990-470f-a638-2bea011e6aed"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f9cec3ec-6dd4-492e-b65b-dcb8c7447fd9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""25caa640-3261-49e0-adb2-fa9381027667"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5caec8a-197f-4079-8ac7-4ca598b083d3"",
                    ""path"": ""<GravitySensor>/gravity"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector3(x=2,y=4,z=0)"",
                    ""groups"": ""Mobile"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""3934e518-8fc1-4288-adb4-2ce36db68d4c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1b29ac63-1eec-4c4f-839a-856e3183399b"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e5c1ef95-6e4f-4c21-a6db-827c97f0835b"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""82d2d64b-fea8-4a0d-8157-a2e951a81df0"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ad303c86-f960-43fc-bec4-ae35bc0e1de2"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5f1d52ad-ed68-4ffd-9284-5fa2ab7a1c47"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61082fa7-7746-4ee3-89b3-7ec545962424"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4239f992-f144-4cc5-abd6-666aea291acb"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Open Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""45d394be-8a6f-43a5-9e46-d6f03da8defc"",
                    ""path"": ""<Gyroscope>/angularVelocity"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mobile"",
                    ""action"": ""Gyro"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ToContinue"",
            ""id"": ""e7106b90-68c6-4fcc-bb8e-d7ad1fb7a9c1"",
            ""actions"": [
                {
                    ""name"": ""SkipAll"",
                    ""type"": ""Button"",
                    ""id"": ""34f33c9b-222b-4818-841c-92e2c9f9c649"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SkipOne"",
                    ""type"": ""Button"",
                    ""id"": ""7e64f5ee-115c-4c8c-80b4-9a2c8453da53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3bcfd030-2894-4dea-997e-de6da65d77fc"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SkipOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b9f8a62b-04f9-4ba6-b9d2-55b6610f8167"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SkipOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9c36b84-8204-45e0-9ac2-dd2314b87ca3"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SkipOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab52b306-1875-4c91-ae31-3b73b2eb57ef"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SkipAll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ea90cf9-123c-463b-a71a-92fe657e0a74"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SkipAll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""137f1722-e66d-4837-8e49-ad40dc3ef0e1"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SkipAll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mobile"",
            ""bindingGroup"": ""Mobile"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gyroscope>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Newaction = m_Menu.FindAction("New action", throwIfNotFound: true);
        m_Menu_Newaction1 = m_Menu.FindAction("New action1", throwIfNotFound: true);
        // In Game
        m_InGame = asset.FindActionMap("In Game", throwIfNotFound: true);
        m_InGame_Shoot = m_InGame.FindAction("Shoot", throwIfNotFound: true);
        m_InGame_Movement = m_InGame.FindAction("Movement", throwIfNotFound: true);
        m_InGame_Boost = m_InGame.FindAction("Boost", throwIfNotFound: true);
        m_InGame_OpenMenu = m_InGame.FindAction("Open Menu", throwIfNotFound: true);
        m_InGame_Gyro = m_InGame.FindAction("Gyro", throwIfNotFound: true);
        // ToContinue
        m_ToContinue = asset.FindActionMap("ToContinue", throwIfNotFound: true);
        m_ToContinue_SkipAll = m_ToContinue.FindAction("SkipAll", throwIfNotFound: true);
        m_ToContinue_SkipOne = m_ToContinue.FindAction("SkipOne", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Newaction;
    private readonly InputAction m_Menu_Newaction1;
    public struct MenuActions
    {
        private @GameControls m_Wrapper;
        public MenuActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Menu_Newaction;
        public InputAction @Newaction1 => m_Wrapper.m_Menu_Newaction1;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction;
                @Newaction1.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction1;
                @Newaction1.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction1;
                @Newaction1.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNewaction1;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
                @Newaction1.started += instance.OnNewaction1;
                @Newaction1.performed += instance.OnNewaction1;
                @Newaction1.canceled += instance.OnNewaction1;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // In Game
    private readonly InputActionMap m_InGame;
    private IInGameActions m_InGameActionsCallbackInterface;
    private readonly InputAction m_InGame_Shoot;
    private readonly InputAction m_InGame_Movement;
    private readonly InputAction m_InGame_Boost;
    private readonly InputAction m_InGame_OpenMenu;
    private readonly InputAction m_InGame_Gyro;
    public struct InGameActions
    {
        private @GameControls m_Wrapper;
        public InGameActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot => m_Wrapper.m_InGame_Shoot;
        public InputAction @Movement => m_Wrapper.m_InGame_Movement;
        public InputAction @Boost => m_Wrapper.m_InGame_Boost;
        public InputAction @OpenMenu => m_Wrapper.m_InGame_OpenMenu;
        public InputAction @Gyro => m_Wrapper.m_InGame_Gyro;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void SetCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterface != null)
            {
                @Shoot.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnShoot;
                @Movement.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnMovement;
                @Boost.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnBoost;
                @OpenMenu.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnOpenMenu;
                @Gyro.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnGyro;
                @Gyro.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnGyro;
                @Gyro.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnGyro;
            }
            m_Wrapper.m_InGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
                @OpenMenu.started += instance.OnOpenMenu;
                @OpenMenu.performed += instance.OnOpenMenu;
                @OpenMenu.canceled += instance.OnOpenMenu;
                @Gyro.started += instance.OnGyro;
                @Gyro.performed += instance.OnGyro;
                @Gyro.canceled += instance.OnGyro;
            }
        }
    }
    public InGameActions @InGame => new InGameActions(this);

    // ToContinue
    private readonly InputActionMap m_ToContinue;
    private IToContinueActions m_ToContinueActionsCallbackInterface;
    private readonly InputAction m_ToContinue_SkipAll;
    private readonly InputAction m_ToContinue_SkipOne;
    public struct ToContinueActions
    {
        private @GameControls m_Wrapper;
        public ToContinueActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SkipAll => m_Wrapper.m_ToContinue_SkipAll;
        public InputAction @SkipOne => m_Wrapper.m_ToContinue_SkipOne;
        public InputActionMap Get() { return m_Wrapper.m_ToContinue; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ToContinueActions set) { return set.Get(); }
        public void SetCallbacks(IToContinueActions instance)
        {
            if (m_Wrapper.m_ToContinueActionsCallbackInterface != null)
            {
                @SkipAll.started -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipAll;
                @SkipAll.performed -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipAll;
                @SkipAll.canceled -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipAll;
                @SkipOne.started -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipOne;
                @SkipOne.performed -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipOne;
                @SkipOne.canceled -= m_Wrapper.m_ToContinueActionsCallbackInterface.OnSkipOne;
            }
            m_Wrapper.m_ToContinueActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SkipAll.started += instance.OnSkipAll;
                @SkipAll.performed += instance.OnSkipAll;
                @SkipAll.canceled += instance.OnSkipAll;
                @SkipOne.started += instance.OnSkipOne;
                @SkipOne.performed += instance.OnSkipOne;
                @SkipOne.canceled += instance.OnSkipOne;
            }
        }
    }
    public ToContinueActions @ToContinue => new ToContinueActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_MobileSchemeIndex = -1;
    public InputControlScheme MobileScheme
    {
        get
        {
            if (m_MobileSchemeIndex == -1) m_MobileSchemeIndex = asset.FindControlSchemeIndex("Mobile");
            return asset.controlSchemes[m_MobileSchemeIndex];
        }
    }
    public interface IMenuActions
    {
        void OnNewaction(InputAction.CallbackContext context);
        void OnNewaction1(InputAction.CallbackContext context);
    }
    public interface IInGameActions
    {
        void OnShoot(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnBoost(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
        void OnGyro(InputAction.CallbackContext context);
    }
    public interface IToContinueActions
    {
        void OnSkipAll(InputAction.CallbackContext context);
        void OnSkipOne(InputAction.CallbackContext context);
    }
}
