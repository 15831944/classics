IxMilia.Classics
================

An MSBuild-based tool to create classics texts with the appropriate vocabulary
on the page.  Output should be in a readable, and potentially pritable form
like PDF.

### Goal

The end goal is to be able to automatically create readable classics texts with
the appropriate vocabulary similar to Pharr's _Aeneid_.  E.g., a page of the
output would look something like this...

---

<center>
  <h2>THE AENEID, 1-4</h2>
  <h3>BOOK 1</h3>

  <table>
    <tr>
      <td>
        &nbsp;&nbsp;&nbsp;&nbsp;_ARMA virumque_ cano, _Troiae qui primus ab oris_<br />
        _Italiam fato_ profugus Lavina_que venit_<br />
        _litora - multum ille et terris_ jactatus _et_ alto<br />
        _vi superum_, saevae memorem _Junonis_ ob _iram_,<br />
      </td>
    </tr>
  </table>

  <hr />

  <table style="border: 0px">
    <tr>
      <td>**altum, i** _n._ the deep (sea) &dagger;</td>
      <td>**memor, oris** mindful, remembering, un-</td>
    </tr>
    <tr>
      <td>**cano, ere, cecini, cantus** sing (of), chant,</td>
      <td>&nbsp;&nbsp;forgetting&dagger;*</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;procalim&dagger;*</td>
      <td>**ob** on account of (_acc._)&dagger;</td>
    </tr>
    <tr>
      <td>**jacto** (1) toss, buffer&dagger;*</td>
      <td>**profugus, a, um** exiled, fugutive&dagger;</td>
    </tr>
    <tr>
      <td>**Lavin(i)us, a, um** Lavinian, of Lavinium&dagger;</td>
      <td>**saevus, a, um** cruel, stern, fierce&dagger;*</td>
    </tr>
  </table>

  1
</center>

---

...with an additional list of the most common (250 or so) words and their
definitions.  Any word in the source text that occurs in the common list will
italicized with other words being romanized.

A vocabulary item on an inividual page will have a dagger (&dagger;) after its
definition if it is the first occurence of that word in the work.  Any words
that occur more than 10 times will also have an asterisk (*).

Ideally this repo will never directly contain the classics texts or the
dictionaries, but will instead be able to fetch them from online resources.
