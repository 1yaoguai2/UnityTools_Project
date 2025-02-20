#region

using System;
using TMPro;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Handles the input UI for adding tasks, including parsing the input and triggering task submission events.
    /// </summary>
    public class TaskInputUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField mInputField;

        private void Awake()
        {
            // Set up the placeholder text and attach the input submission listener.
            mInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Description-MMDD";
            mInputField.onSubmit.AddListener(_ => OnInputComplete());
        }

        // Event triggered when a task is successfully submitted with its description and deadline.
        public event Action<string, DateTime> OnTaskSubmitted;

        /// <summary>
        ///     Handles the completion of the input field.
        ///     Validates the input and triggers the task submission event if valid.
        /// </summary>
        private void OnInputComplete()
        {
            if (TryParseInput(mInputField.text, out var description, out var deadline))
            {
                // Trigger the task submission event with the parsed description and deadline.
                OnTaskSubmitted?.Invoke(description, deadline);
                ClearInput();
            }
            else
            {
                // Display an error message if the input format is invalid.
                mInputField.text = "Format wrong, Use format:Description-MMDD";
            }
        }

        /// <summary>
        ///     Attempts to parse the input text into a task description and deadline.
        /// </summary>
        /// <param name="_input">The input string in the format: "description-MMDD".</param>
        /// <param name="_description">The parsed task description.</param>
        /// <param name="_deadline">The parsed deadline as a DateTime object.</param>
        /// <returns>True if parsing succeeds, otherwise false.</returns>
        private bool TryParseInput(string _input, out string _description, out DateTime _deadline)
        {
            _description = string.Empty;
            _deadline = DateTime.Now;

            // Find the last '-' separator in the input string.
            var separatorIndex = _input.LastIndexOf('-');
            if (separatorIndex <= 0 || separatorIndex == _input.Length - 1)
                return false;

            // Extract and trim the description and date parts.
            _description = _input[..separatorIndex].Trim();
            var dateStr = _input[(separatorIndex + 1)..].Trim();

            // Validate and parse the MMDD date format.
            if (dateStr.Length != 4 || !int.TryParse(dateStr[..2], out var month) ||
                !int.TryParse(dateStr[2..], out var day))
                return false;

            try
            {
                // Create a DateTime object for the parsed month and day. If the deadline is in the past, move it to the next year.
                _deadline = new DateTime(DateTime.Now.Year, month, day);
                if (_deadline < DateTime.Now)
                    _deadline = _deadline.AddYears(1);
                return true;
            }
            catch
            {
                // Return false if the date is invalid.
                return false;
            }
        }

        /// <summary>
        ///     Clears the input field.
        /// </summary>
        private void ClearInput()
        {
            mInputField.text = string.Empty;
        }
    }
}