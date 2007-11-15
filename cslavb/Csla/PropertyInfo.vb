﻿''' <summary>
''' Maintains metadata about a property.
''' </summary>
Public Class PropertyInfo(Of T)

  Implements Core.IPropertyInfo

  Friend Sub New(ByVal name As String)

    mName = name

  End Sub

  Friend Sub New(ByVal name As String, ByVal defaultValue As T)

    Me.New(name)
    mDefaultValue = defaultValue

  End Sub

  Friend Sub New(ByVal name As String, ByVal friendlyName As String)

    Me.New(name)
    mFriendlyName = friendlyName

  End Sub

  Friend Sub New(ByVal name As String, ByVal friendlyName As String, ByVal defaultValue As T)

    Me.New(name, defaultValue)
    mFriendlyName = friendlyName

  End Sub

  Private mName As String
  ''' <summary>
  ''' Gets the property name value.
  ''' </summary>
  Public ReadOnly Property Name() As String Implements Core.IPropertyInfo.Name
    Get
      Return mName
    End Get
  End Property

  Private mType As Type
  ''' <summary>
  ''' Gets the type of the property.
  ''' </summary>
  Public ReadOnly Property Type() As Type Implements Core.IPropertyInfo.Type
    Get
      Return GetType(T)
    End Get
  End Property

  Private mFriendlyName As String
  ''' <summary>
  ''' Gets the friendly display name
  ''' for the property.
  ''' </summary>
  ''' <remarks>
  ''' If no friendly name was provided, the
  ''' property name itself is returned as a
  ''' result.
  ''' </remarks>
  Public ReadOnly Property FriendlyName() As String Implements Core.IPropertyInfo.FriendlyName
    Get
      If mFriendlyName IsNot Nothing Then
        Return mFriendlyName

      Else
        Return mName
      End If
    End Get
  End Property

  Private mDefaultValue As T = Nothing
  Public ReadOnly Property DefaultValue() As T
    Get
      Return mDefaultValue
    End Get
  End Property

End Class
