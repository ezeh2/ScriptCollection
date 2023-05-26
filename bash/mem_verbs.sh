#!/bin/bash

# Read verbs and translations from a file
read_verbs_and_translations() {

    echo "file: $file,  delimiter: $delimiter"

    # Check if the file exists
    if [[ ! -f "$file" ]]; then
        echo "File '$file' does not exist."
        return 1
    fi

    # Read the file line by line, using semicolon as the delimiter
    while IFS="$delimiter" read -r verb translation; do
        verbs+=("$verb")
        translations+=("$translation")
    done < "$file"

}

# Function to display a random verb and its translation
display_random_verb() {
    # Select a random index from the verbs array
    random_index=$((RANDOM % ${#verbs[@]}))
    random_verb=${verbs[$random_index]}
    random_translation=${translations[$random_index]}
    
    echo "meaning of this word: $random_verb ?"
    read -r -p ""
    echo "Translation: $random_translation"
    echo ""
}

file=$1
delimiter=$2

read_verbs_and_translations

# Main loop to display verbs
while true; do
#    echo "Press enter to see a verb, or 'q' to quit."
#    read -r input
#
#    if [[ "$input" == "q" ]]; then
#        break
#    fi

    display_random_verb
done

