namespace Client.Types

type Input<'t> =
    | NotEditing
    | Editing of 't
    | Submitted of 't