﻿public interface ISavedProgress : ISavedProgressReader
{
    public void SaveProgress(PlayerProgress persistentPlayerProgress);
}