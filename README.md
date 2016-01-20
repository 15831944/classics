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
        &nbsp;&nbsp;&nbsp;&nbsp;<i>ARMA virumque</i> cano, <i>Troiae qui primus ab oris</i><br />
        <i>Italiam fato</i> profugus Lavina<i>que venit</i><br />
        <i>litora - multum ille et terris</i> jactatus <i>et</i> alto<br />
        <i>vi superum</i>, saevae memorem <i>Junonis</i> ob <i>iram</i>,<br />
      </td>
    </tr>
  </table>

  <hr />

  <table style="border: 0px">
    <tr>
      <td><b>altum, i</b> _n._ the deep (sea) &dagger;</td>
      <td><b>memor, oris</b> mindful, remembering, un-</td>
    </tr>
    <tr>
      <td><b>cano, ere, cecini, cantus</b> sing (of), chant,</td>
      <td>&nbsp;&nbsp;forgetting&dagger;&#42;</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;procalim&dagger;&#42;</td>
      <td><b>ob</b> on account of (_acc._)&dagger;</td>
    </tr>
    <tr>
      <td><b>jacto</b> (1) toss, buffer&dagger;&#42;</td>
      <td><b>profugus, a, um</b> exiled, fugutive&dagger;</td>
    </tr>
    <tr>
      <td><b>Lavin(i)us, a, um</b> Lavinian, of Lavinium&dagger;</td>
      <td><b>saevus, a, um</b> cruel, stern, fierce&dagger;&#42;</td>
    </tr>
  </table>

  1
</center>

---

...with an additional list of the most common (250 or so) words and their
definitions.  Any word in the source text that occurs in the common list will
be italicized with other words being romanized.

A vocabulary item on an inividual page will have a dagger (&dagger;) after its
definition if it is the first occurence of that word in the work.  Any words
that occur more than 10 times will also have an asterisk (*).

Ideally this repo will never directly contain the classics texts or the
dictionaries, but will instead be able to fetch them from online resources.

### Usage

``` bash
msbuild Build.proj
```
