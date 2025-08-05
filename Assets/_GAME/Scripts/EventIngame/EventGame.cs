using IPS;

public struct PreStartGameEvent : IEventParam { }
public struct StartGameEvent : IEventParam { }
public struct EndGameEvent : IEventParam { public bool IsWin; }
public struct PauseGameEvent : IEventParam { }
public struct ContinueGameEvent : IEventParam { }
public struct ReviveGameEvent : IEventParam { }