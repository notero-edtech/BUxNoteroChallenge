SetBestFitRect(this RectTransform)
----------------------------------

This will only work if we : 

1) move character mesh for the offset we get from GetBestRitRect().x, .y
2) in order to visually keep the character on the same place we need to move anchoredPosition for the same offset in oposite direction

NOTE: we need to store this offset in NSText in order to have all positiononing correct, which means every object with BestFitRect needs to take this offset into positioning account
