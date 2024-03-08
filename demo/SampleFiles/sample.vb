Private Sub Command1_Click()
Randomize Timer
n = Int(1 + Rnd * 6)
For i = 0 To 6
Shape1(i).Visible = False
Next


If n = 1 Then
Shape1(3).Visible = True
End If


If n = 2 Then
Shape1(2).Visible = True
Shape1(4).Visible = True
End If

If n = 3 Then
Shape1(2).Visible = True
Shape1(3).Visible = True
Shape1(4).Visible = True
End If
If n = 4 Then
Shape1(0).Visible = True
Shape1(2).Visible = True
Shape1(4).Visible = True
Shape1(6).Visible = True
End If


If n = 5 Then
Shape1(0).Visible = True
Shape1(2).Visible = True
Shape1(3).Visible = True
Shape1(4).Visible = True
Shape1(6).Visible = True

End If
If n = 6 Then
Shape1(0).Visible = True
Shape1(1).Visible = True
Shape1(2).Visible = True
Shape1(4).Visible = True
Shape1(5).Visible = True
Shape1(6).Visible = True

End If
End Sub