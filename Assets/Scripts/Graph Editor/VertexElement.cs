using UnityEngine;
using UnityEngine.UIElements;

public class VertexElement : VisualElement
{
    public Vertex vertex;
    private Label nameLabel;
    private System.Action<VertexElement> onShiftClick;

    public VertexElement(Vertex vertex, System.Action<VertexElement> onShiftClick)
    {
        this.vertex = vertex;
        this.onShiftClick = onShiftClick;

        style.position = Position.Absolute;
        style.left = vertex.position.x;
        style.top = vertex.position.y;
        style.width = 10;
        style.height = 10;
        style.backgroundColor = Color.red;

        nameLabel = new Label(vertex.name)
        {
            style = { position = Position.Absolute, left = 25, top = 0 }
        };
        Add(nameLabel);

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseMoveEvent>(OnMouseMove);
        RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.clickCount == 2)
        {
            StartEditingName();
        }
        else if (evt.shiftKey)
        {
            onShiftClick?.Invoke(this);
        }
        else
        {
            // Select the vertex and allow dragging
            (parent as GraphCanvas).selectedVertex = this;
            this.CaptureMouse();
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (this.HasMouseCapture())
        {
            Vector2 newPosition = new Vector2(
                style.left.value.value + evt.mouseDelta.x,
                style.top.value.value + evt.mouseDelta.y
            );

            style.left = newPosition.x;
            style.top = newPosition.y;
            vertex.position = newPosition;

            nameLabel.style.left = 25;
            nameLabel.style.top = 0;

            (parent as GraphCanvas)?.MarkDirtyRepaint();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (this.HasMouseCapture())
        {
            this.ReleaseMouse();
        }
    }

    private void StartEditingName()
    {
        var textField = new TextField
        {
            value = vertex.name,
            style = { position = Position.Absolute, left = 25, top = 0 }
        };

        textField.RegisterCallback<FocusOutEvent>(evt =>
        {
            SetName(textField.value);
            Remove(textField);
            Add(nameLabel);
        });

        textField.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Return)
            {
                SetName(textField.value);
                Remove(textField);
                Add(nameLabel);
            }
        });

        Remove(nameLabel);
        Add(textField);
        textField.Focus();
    }

    public void SetName(string newName)
    {
        if (string.IsNullOrEmpty(newName)) return;
        vertex.name = newName;
        nameLabel.text = newName;
    }

    // Visual feedback for selection
    public void SetSelected(bool isSelected)
    {
        style.borderBottomWidth = isSelected ? 2 : 0;
        style.borderBottomColor = isSelected ? Color.blue : Color.clear;
    }
}