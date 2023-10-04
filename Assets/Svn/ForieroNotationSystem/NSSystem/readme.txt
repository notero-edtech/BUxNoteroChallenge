DIVISIONS
    The duration element is an integer that represents a note’s duration in terms of divisions per quarter note. 
    Since our example has 24 divisions per quarter note, a quarter note has a duration of 24. 
    The eighth-note triplets have a duration of 8, while the eighth notes have a duration of 12.
 
POSITIONING
    The default-x and default-y attributes provide more precise positioning, and use units of tenths of interline space.
    The default-x attribute is measured from the start of the current measure (for other elements, it is measured from the left-hand side of the note or the musical position within the bar.)
    The default-y attribute is measured from the top barline of the staff. 
 
STEMS
    Stem direction is represented with the stem element, whose value can be up, down, none, or double.
    For up and down stems, the default-y attribute represents where the stem ends, measured in tenths of interline space from the top line of the staff.
 
BEAMS
    Beams are represented by beam elements. Their value can be begin, continue, end, forward hook, and backward hook.
    Each element has a beam-level attribute which ranges from 1 to 6 for eighth-note to 256th-note beams.

GRACE, CUE

BAR LINES


    